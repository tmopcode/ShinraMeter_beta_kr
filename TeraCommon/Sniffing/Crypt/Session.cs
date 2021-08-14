﻿// Unknown Author and License

using System;

namespace Tera.Sniffing.Crypt
{
    public class Session {
        public byte[] ClientKey1 = new byte[128];
        public byte[] ClientKey2 = new byte[128];
        public byte[] DecryptKey = new byte[128];
        protected Cryptor Decryptor;

        public byte[] EncryptKey = new byte[128];
        protected Cryptor Encryptor;
        public byte[] ServerKey1 = new byte[128];
        public byte[] ServerKey2 = new byte[128];

        public byte[] TmpKey1 = new byte[128];
        public byte[] TmpKey2 = new byte[128];


        public Session(byte[] clientKey1, byte[] clientKey2, byte[] serverKey1, byte[] serverKey2, bool newshifts = true) {
            ClientKey1 = clientKey1;
            ClientKey2 = clientKey2;
            ServerKey1 = serverKey1;
            ServerKey2 = serverKey2;
            TmpKey1 = Utils.ShiftKey(ServerKey1, newshifts ? 67 : 31);

            TmpKey2 = Utils.XorKey(TmpKey1, ClientKey1);

            TmpKey1 = Utils.ShiftKey(ClientKey2, newshifts ? 29 : 17, false);

            DecryptKey = Utils.XorKey(TmpKey1, TmpKey2);

            Decryptor = new Cryptor(DecryptKey);

            TmpKey1 = Utils.ShiftKey(ServerKey2, newshifts ? 41 : 79);

            Decryptor.ApplyCryptor(TmpKey1, 128);
            EncryptKey = new byte[128];
            Buffer.BlockCopy(TmpKey1, 0, EncryptKey, 0, 128);

            Encryptor = new Cryptor(EncryptKey);
        }

        public void Encrypt(byte[] data)
        {
            Encryptor.ApplyCryptor(data, data.Length);
        }

        public void Decrypt(byte[] data)
        {
            Decryptor.ApplyCryptor(data, data.Length);
        }
    }
}