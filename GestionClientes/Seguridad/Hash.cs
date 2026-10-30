using System;
using System.Security.Cryptography;

namespace GestionClientes.Seguridad
{
    public static class Hash
    {
        private const int Iteraciones = 100000;
        private const int LargoHash = 32;
        private const int LargoSal = 16;

        // crea una sal nueva y aleatoria para un usuario
        public static string GenerarSal()
        {
            byte[] sal = new byte[LargoSal];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(sal);
            }

            return ABytes(sal);
        }

        // calcula el hash de la contraseña usando la sal del usuario
        public static string GenerarHash(string password, string sal)
        {
            byte[] bytesSal = AHex(sal);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, bytesSal, Iteraciones, HashAlgorithmName.SHA256))
            {
                return ABytes(pbkdf2.GetBytes(LargoHash));
            }
        }

        // compara lo que escribio el usuario contra lo que esta guardado
        public static bool Verificar(string password, string sal, string hashGuardado)
        {
            string hashCalculado = GenerarHash(password, sal);
            return Comparar(hashCalculado, hashGuardado);
        }

        // comparacion en tiempo constante para no filtrar informacion por el tiempo de respuesta
        private static bool Comparar(string a, string b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            if (a.Length != b.Length)
            {
                return false;
            }

            int diferencia = 0;

            for (int i = 0; i < a.Length; i++)
            {
                diferencia |= a[i] ^ b[i];
            }

            return diferencia == 0;
        }

        private static string ABytes(byte[] datos)
        {
            return BitConverter.ToString(datos).Replace("-", "");
        }

        private static byte[] AHex(string texto)
        {
            byte[] bytes = new byte[texto.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(texto.Substring(i * 2, 2), 16);
            }

            return bytes;
        }
    }
}
