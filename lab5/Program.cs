using System;
using System.Collections;
using System.IO;

namespace lab5
{
    class Program
    {
        public static byte[] ChangeBytes(int[][] sbox, byte[] start, byte[] getbit, int[] permutation, bool state)
        {
            byte[] result = new byte[start.Length];
            bool[] arr = new bool[start.Length * 8];
            for (int i = 0; i < start.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((start[i] & getbit[j]) == 0)
                    {
                        arr[j + i * 8] = false;
                    }
                    else
                    {
                        arr[j + i * 8] = true;
                    }
                }
            }
            BitArray oldBits = new BitArray(arr);
            BitArray newBits = new BitArray(arr);
            newBits.SetAll(false);
            for (int i = 0; i < start.Length * 8 / 6; i++)
            {
                if (state)
                {
                    newBits[i * 6] = oldBits.Get(permutation[0] + i * 6);
                    newBits[1 + i * 6] = oldBits.Get(permutation[1] + i * 6);

                    int str = 0;
                    if (newBits.Get(i * 6)) str += 2;
                    if (newBits.Get(1 + i * 6)) str += 1;

                    BitArray box = new BitArray(4);
                    box[0] = oldBits.Get(permutation[2] + i * 6);
                    box[1] = oldBits.Get(permutation[3] + i * 6);
                    box[2] = oldBits.Get(permutation[4] + i * 6);
                    box[3] = oldBits.Get(permutation[5] + i * 6);

                    int stb = 0;
                    if (box.Get(0)) stb += 8;
                    if (box.Get(1)) stb += 4;
                    if (box.Get(2)) stb += 2;
                    if (box.Get(3)) stb += 1;

                    box = new BitArray(GetBoolSBox(sbox, getbit, str, stb, state));
                    newBits[2 + i * 6] = box.Get(0);
                    newBits[3 + i * 6] = box.Get(1);
                    newBits[4 + i * 6] = box.Get(2);
                    newBits[5 + i * 6] = box.Get(3);
                }
                else
                {
                    int str = 0;
                    if (oldBits.Get(i * 6)) str += 2;
                    if (oldBits.Get(1 + i * 6)) str += 1;
                    BitArray box = new BitArray(4);
                    box[0] = oldBits.Get(2 + i * 6);
                    box[1] = oldBits.Get(3 + i * 6);
                    box[2] = oldBits.Get(4 + i * 6);
                    box[3] = oldBits.Get(5 + i * 6);
                    int value = 0;
                    if (box.Get(0)) value += 8;
                    if (box.Get(1)) value += 4;
                    if (box.Get(2)) value += 2;
                    if (box.Get(3)) value += 1;
                    int stb = FindStb(sbox, str, value);
                    box = new BitArray(GetBoolSBox(sbox, getbit, str, stb, state));
                    oldBits[2 + i * 6] = box[0];
                    oldBits[3 + i * 6] = box[1];
                    oldBits[4 + i * 6] = box[2];
                    oldBits[5 + i * 6] = box[3];
                    newBits[i * 6] = oldBits.Get(permutation[0] + i * 6);
                    newBits[1 + i * 6] = oldBits.Get(permutation[1] + i * 6);
                    newBits[2 + i * 6] = oldBits.Get(permutation[2] + i * 6);
                    newBits[3 + i * 6] = oldBits.Get(permutation[3] + i * 6);
                    newBits[4 + i * 6] = oldBits.Get(permutation[4] + i * 6);
                    newBits[5 + i * 6] = oldBits.Get(permutation[5] + i * 6);
                }
            }
            for (int i = 0; i < start.Length; i++)
            {
                result[i] = ConvertToByte(newBits, i);
            }
            return result;
        }

        public static byte ConvertToByte(BitArray bits, int i)
        {
            if (bits.Count % 8 != 0)
            {
                throw new ArgumentException("illegal number of bits");
            }
            byte b = 0;
            if (bits.Get(7 + i * 8)) b++;
            if (bits.Get(6 + i * 8)) b += 2;
            if (bits.Get(5 + i * 8)) b += 4;
            if (bits.Get(4 + i * 8)) b += 8;
            if (bits.Get(3 + i * 8)) b += 16;
            if (bits.Get(2 + i * 8)) b += 32;
            if (bits.Get(1 + i * 8)) b += 64;
            if (bits.Get(0 + i * 8)) b += 128;
            return b;
        }

        public static int FindStb(int[][] sbox, int str, int value)
        {
            int stb = 0;
            for (int i = 0; i < 16; ++i)
            {
                if (sbox[str][i] == value)
                    stb = i;
            }
            return stb;
        }

        public static bool[] GetBoolSBox(int[][] sbox, byte[] getbit, int str, int stb, bool state)
        {
            bool[] result = new bool[4];
            int a = 0;
            if (state)
            {
                a = sbox[str][stb];
            }
            else
            {
                a = stb;
            }
            for (int i = 0; i < 4; i++)
            {
                if ((a & getbit[i + 4]) == 0)
                {
                    result[i] = false;
                }
                else
                {
                    result[i] = true;
                }
            }
            return result;
        }

        static void Main(string[] args)
        {
            int[][] sbox = new int[4][];
            sbox[0] = new int[16] { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 };
            sbox[1] = new int[16] { 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 };
            sbox[2] = new int[16] { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 };
            sbox[3] = new int[16] { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 };
            byte[] getbit = new byte[8] { 0x80, 0x40, 0x20, 0x10, 0x8, 0x4, 0x2, 0x1 };
            int[] permutation = new int[6] { 2, 1, 0, 3, 4, 5 };
            int[] backPermutation = new int[6] { 2, 1, 0, 3, 4, 5 };
            char menu = 'a';
            while (menu != '1' && menu != '2' && menu != '3')
            {
                Console.WriteLine("1. Encryption \n2. Decryption \n3. Exit");
                Console.Write(">>");
                menu = Console.ReadKey().KeyChar;
                if (menu != '1' && menu != '2' && menu != '3')
                {
                    Console.WriteLine("\nEncorrect command. Choose 1, 2 or 3;\n");
                }
            }
            if (menu == '3')
                return;
            Console.Clear();
            if (menu == '1')
                Console.WriteLine("Path to file for encryption:");
            else
                Console.WriteLine("Path to file for decryption:");


            string pathForOpen = Console.ReadLine();
            bool state;
            if (menu == '1')
            {
                Console.WriteLine("Name of file to save encryption text:");
                state = true;
            }
            else
            {
                Console.WriteLine("Name of file to save decryption text:");
                state = false;
            }
            string pathForClose = pathForOpen.Substring(0, pathForOpen.LastIndexOf('\\') + 1) + Console.ReadLine();
            FileStream fileOpen = new FileStream(pathForOpen, FileMode.Open, FileAccess.Read);
            FileStream fileClose = new FileStream(pathForClose, FileMode.Create, FileAccess.Write);
            byte[] buf = new byte[3];
            long lenght = fileOpen.Length / 3;
            long end = fileOpen.Length % 3;
            fileOpen.Position = 0;
            fileClose.Position = 0;
            long counter = 0;
            byte[] result = new byte [3];
            for (int i = 0; i < lenght + 1; i++)
            {
                buf = new byte[] { 0, 0, 0 };
                if (counter != lenght)
                {
                    buf[0] = (byte)fileOpen.ReadByte();
                    buf[1] = (byte)fileOpen.ReadByte();
                    buf[2] = (byte)fileOpen.ReadByte();
                }
                else
                {
                    if (end == 1)
                        fileClose.WriteByte((byte)fileOpen.ReadByte());
                    if (end == 2)
                    {
                        fileClose.WriteByte((byte)fileOpen.ReadByte());
                        fileClose.WriteByte((byte)fileOpen.ReadByte());
                    }
                }
                if (menu == '1' && counter != lenght)
                {
                    result = ChangeBytes(sbox, buf, getbit, permutation, state);
                }
                else if (menu == '2' && counter != lenght)
                {
                    result = ChangeBytes(sbox, buf, getbit, backPermutation, state);
                }
                if (counter != lenght)
                {
                    fileClose.WriteByte(result[0]);
                    fileClose.WriteByte(result[1]);
                    fileClose.WriteByte(result[2]);
                } 
                counter++;
            }
            fileOpen.Dispose();
            fileOpen.Close();
            fileClose.Dispose();
            fileClose.Close();
        }
    }
}
