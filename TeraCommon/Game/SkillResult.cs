﻿using System;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Tera.Game.Messages;

namespace Tera.Game
{
    public class SkillResult
    {
        public SkillResult(EachSkillResultServerMessage message, EntityTracker entityRegistry,
            PlayerTracker playerTracker, SkillDatabase skillDatabase, PetSkillDatabase petSkillDatabase = null, AbnormalityTracker abnormalityTracker=null)
        {
            Time = message.Time;
            Amount = message.Amount;
            IsCritical = message.IsCritical;
            IsHp = message.IsHp;
            IsHeal = message.IsHeal;
            SkillId = message.SkillId;
            Abnormality = false;

            Source = entityRegistry.GetOrPlaceholder(message.Source);
            Target = entityRegistry.GetOrPlaceholder(message.Target);
            if (Source is PlaceHolderEntity && (Target as NpcEntity)?.Info.Boss == true)
                Source = playerTracker.GetUnknownPlayer() ?? Source;  //track unknown damage dealt to bosses like in raid-30
            if (abnormalityTracker?.AbnormalityExist(message.Target, 950187) ?? false) Amount=0; //fix raid-30 bug with 1kkk damage after shield
            var userNpc = UserEntity.ForEntity(Source);
            var npc = (NpcEntity) userNpc["source"];
            var rootSource = userNpc["root_source"];
            var sourceUser = rootSource as UserEntity; // Attribute damage dealt by owned entities to the owner
            var targetUser = Target as UserEntity; // But don't attribute damage received by owned entities to the owner

            if (sourceUser != null)
            {
                Skill = skillDatabase.Get(sourceUser, message);
                if (Skill == null && npc != null)
                {
                    Skill = petSkillDatabase?.GetOrNull(npc.Info, SkillId) ?? new UserSkill(message.SkillId, sourceUser.RaceGenderClass, npc.Info.Name, null,
                        "", skillDatabase.GetSkillByPetName(npc.Info.Name, sourceUser.RaceGenderClass)?.IconName ?? "", npc.Info);
                }
                SourcePlayer = playerTracker.Get(sourceUser.ServerId, sourceUser.PlayerId);
                if ((SkillId == 51001 || SkillId == 51011 || SkillId == 51021)
                    && (abnormalityTracker?.AbnormalityExist(Source.Id, 10152050) ?? false)) {//gunner burstfire buff detection
                    SkillId = SkillId + abnormalityTracker.AbnormalityStorage.Get(SourcePlayer).Times.Last(x => x.Key.Id == 10152050).Value.LastStack();
                    Skill = skillDatabase.GetOrNull(sourceUser, SkillId);
                }
                if (Skill == null)
                    Skill = new UserSkill(message.SkillId, sourceUser.RaceGenderClass, "Unknown");

            }
            if (targetUser != null)
            {
                TargetPlayer = playerTracker.Get(targetUser.ServerId, targetUser.PlayerId);
            }
            if (Source == null || Target == null || Source is PlaceHolderEntity || Target is PlaceHolderEntity) return;//fix error on skills from or dealt to unknown entities
            Source.Position = Source.Position.MoveForvard(Source.Finish, Source.Speed,
                message.Time.Ticks - Source.StartTime);
            if (Source.EndTime > 0 && Source.EndTime <= Source.StartTime)
            {
                Source.Heading = Source.EndAngle;
                Source.EndTime = 0;
            }
            Target.Position = Target.Position.MoveForvard(Target.Finish, Target.Speed,
                message.Time.Ticks - Target.StartTime);
            if (Target.EndTime > 0 && Target.EndTime <= Target.StartTime)
            {
                Target.Heading = Target.EndAngle;
                Target.EndTime = 0;
            }
            if (SourcePlayer != null && npc != null) HitDirection = HitDirection.Pet;
            else
            {
                if (Source is ProjectileEntity && (Skill?.Boom ?? true)) HitDirection = Source.Position.GetHeading(Target.Position).HitDirection(Target.Heading);
                else { HitDirection = Angle.HitDirection(rootSource.Position, rootSource.Heading, Target.Position, Target.Heading); }
            }

            if ((SourcePlayer?.Class==PlayerClass.Archer) && (abnormalityTracker?.AbnormalityExist(sourceUser.Id, 601600) ?? false)) HitDirection=HitDirection.Back;
            //Debug.WriteLine(HitDirection);
            HitDirection = HitDirection & ~(HitDirection.Left | HitDirection.Right);
        }

        public SkillResult(long amount, bool isCritical, bool isHp, bool isHeal, HotDot hotdot, EntityId source,
            EntityId target, DateTime time,
            EntityTracker entityRegistry, PlayerTracker playerTracker)
        {
            Time = time;
            Amount = isHp ? Math.Abs(amount) : amount;
            IsCritical = isCritical;
            IsHp = isHp;
            IsHeal = isHeal;
            SkillId = hotdot.Id;
            Abnormality = true;

            Source = entityRegistry.GetOrPlaceholder(source);
            Target = entityRegistry.GetOrPlaceholder(target);
            var userNpc = UserEntity.ForEntity(Source);
            var sourceUser = userNpc["root_source"] as UserEntity; // Attribute damage dealt by owned entities to the owner
            var targetUser = Target as UserEntity; // But don't attribute damage received by owned entities to the owner

            var pclass = PlayerClass.Common;
            if (sourceUser != null)
            {
                SourcePlayer = playerTracker.Get(sourceUser.ServerId, sourceUser.PlayerId);
                pclass = SourcePlayer.RaceGenderClass.Class;
            }
            Skill = new UserSkill(hotdot.Id, pclass,
                hotdot.Name, "DOT", null, hotdot.IconName);

            if (targetUser != null)
            {
                TargetPlayer = playerTracker.Get(targetUser.ServerId, targetUser.PlayerId);
            }
            HitDirection = HitDirection.Dot;
        }

        public HitDirection HitDirection { get; private set; }
        public DateTime Time { get; private set; }
        public bool Abnormality { get; }
        public long Amount { get; }
        public Entity Source { get; }
        public Entity Target { get; }
        public bool IsCritical { get; private set; }
        public bool IsHp { get; }
        public bool IsHeal { get; }

        public int SkillId { get; }
        public Skill Skill { get; }
        public string SkillName => Skill?.Name ?? SkillId.ToString();
        public string SkillShortName => Skill?.ShortName ?? SkillId.ToString();

        public string SkillNameDetailed
            =>
                $"{Skill?.Name ?? SkillId.ToString()} {(IsChained != null ? (bool) IsChained ? "[C]" : null : null)} {(string.IsNullOrEmpty(Skill?.Detail) ? null : $"({Skill.Detail})")}"
                    .Replace("  ", " ");

        public bool? IsChained => Skill?.IsChained;

        public long Damage => IsHeal || !IsHp ? 0 : Amount;

        public long Heal => IsHp && IsHeal ? Amount : 0;
        public long Mana => !IsHp ? Amount : 0;


        public Player SourcePlayer { get; }
        public Player TargetPlayer { get; private set; }


        public static Skill GetSkill(Entity source, NpcInfo pet, int skillid, bool hotdot, EntityTracker entityRegistry,
            SkillDatabase skillDatabase, HotDotDatabase hotdotDatabase, PetSkillDatabase petSkillDatabase = null)
        {
            if (hotdot)
            {
                var hotdotskill = hotdotDatabase.Get(skillid);
                return new Skill(skillid, hotdotskill.Name, null, "", hotdotskill.IconName, null, true);
            }

            var sourceUser = source as UserEntity;
            if (sourceUser == null) return null;
            var skill = skillDatabase.GetOrNull(sourceUser.RaceGenderClass, skillid);
            if (skill != null)return skill;
            if (pet == null) return new UserSkill(skillid, sourceUser.RaceGenderClass, "Unknown");
            skill = petSkillDatabase?.GetOrNull(pet, skillid) ??
                new UserSkill(skillid, sourceUser.RaceGenderClass, pet.Name, null,"",
                skillDatabase.GetSkillByPetName(pet.Name, sourceUser.RaceGenderClass)?.IconName ?? "", pet);
            return skill;
        }

        public bool IsValid(DateTime? firstAttack = null)
        {
            return IsHp && //no mana
                   (firstAttack != null || (!IsHeal && Amount > 0)) &&
                   //only record first hit is it's a damage hit (heals occurring outside of fights)
                   !(Target.Equals(Source) && !IsHeal && Amount > 0);
            //disregard damage dealt to self (gunner self destruct)
        }

        public override string ToString()
        {
            return $"{SkillName}({SkillId}) [{Amount}]";
        }
    }
}