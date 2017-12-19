using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynacorVM
{
    public class VM
    {
        public ushort[] Memory { get; set; }
        public ushort[] Registers { get; set; }
        public Stack<ushort> Stack { get; set; }
        public ushort PC { get; set; }

        public VM()
        {
            Memory = new ushort[ushort.MaxValue];
            Registers = new ushort[8];
            Stack = new Stack<ushort>();
            PC = 0;
        }

        public void Read(string Path)
        {
            for (int i = 0; i < Memory.Length; i++)
            {
                Memory[i] = 0;
            }
            using (BinaryReader BR = new BinaryReader(new FileStream(Path, FileMode.Open)))
            {
                List<ushort> DataTemp = new List<ushort>();
                while (BR.BaseStream.Position != BR.BaseStream.Length)
                {
                    DataTemp.Add(BR.ReadUInt16());
                }
                for (int i = 0; i < DataTemp.Count; i++)
                {
                    Memory[i] = DataTemp[i];
                }
            }
        }

        public void Run()
        {
            while(true)
            {
                PC++;
                switch (Memory[PC - 1])
                {
                    //HALT
                    case 0:
                        {
                            return;
                        }
                        break;
                    
                    //SET a b
                    case 1:
                        {
                            ushort A = Argument();
                            ushort B = Argument();

                            Registers[GetRegister(A)] = GetValue(B);
                        }
                        break;
                    
                    //PUSH a
                    case 2:
                        {
                            ushort A = Argument();

                            Stack.Push(GetValue(A));
                        }
                        break;
                   
                    //POP a
                    case 3:
                        {
                            ushort A = Argument();

                            Registers[GetRegister(A)] = Stack.Pop();
                        }
                        break;
                    
                    //EQ a b c 
                    case 4:
                        {
                            ushort A = Argument();
                            ushort B = Argument();
                            ushort C = Argument();

                            if (GetValue(B) == GetValue(C))
                            {
                                Registers[GetRegister(A)] = 1;
                            }
                            else
                            {
                                Registers[GetRegister(A)] = 0;
                            }
                        }
                        break;
                    
                    //GT a b c
                    case 5:
                        {
                            ushort A = Argument();
                            ushort B = Argument();
                            ushort C = Argument();

                            if (GetValue(B) > GetValue(C))
                            {
                                Registers[GetRegister(A)] = 1;
                            }
                            else
                            {
                                Registers[GetRegister(A)] = 0;
                            }
                        }
                        break;

                    //JMP
                    case 6:
                        {
                            ushort A = Argument();
                            PC = GetValue(A);
                        }
                        break;

                    //JT a b
                    case 7:
                        {
                            ushort A = Argument();
                            ushort B = Argument();

                            if (GetValue(A) != 0)
                            {
                                PC = GetValue(B);
                            }
                        }
                        break;

                    //JF a b
                    case 8:
                        {
                            ushort A = Argument();
                            ushort B = Argument();

                            if (GetValue(A) == 0)
                            {
                                PC = GetValue(B);
                            }
                        }
                        break;
                    
                    //ADD a b c
                    case 9:
                        {
                            ushort A = Argument();
                            ushort B = Argument();
                            ushort C = Argument();

                            Registers[GetRegister(A)] = (ushort)((GetValue(B) + GetValue(C)) % 32768);
                        }
                        break;

                    //MULT a b c
                    case 10:
                        {
                            ushort A = Argument();
                            ushort B = Argument();
                            ushort C = Argument();

                            Registers[GetRegister(A)] = (ushort)((GetValue(B) * GetValue(C)) % 32768);
                        }
                        break;

                    //MOD a b c
                    case 11:
                        {
                            ushort A = Argument();
                            ushort B = Argument();
                            ushort C = Argument();

                            Registers[GetRegister(A)] = (ushort)(GetValue(B) % GetValue(C));
                        }
                        break;

                    //AND a b c
                    case 12:
                        {
                            ushort A = Argument();
                            ushort B = Argument();
                            ushort C = Argument();

                            Registers[GetRegister(A)] = (ushort)(GetValue(B) & GetValue(C));
                        }
                        break;
                    
                    //OR a b c 
                    case 13:
                        {
                            ushort A = Argument();
                            ushort B = Argument();
                            ushort C = Argument();

                            Registers[GetRegister(A)] = (ushort)(GetValue(B) | GetValue(C));
                        }
                        break;
                    
                    //NOT a b
                    case 14:
                        {
                            ushort A = Argument();
                            ushort B = Argument();

                            Registers[GetRegister(A)] = (ushort)(~GetValue(B) & 0b00000000000000000111111111111111);
                        }
                        break;

                    //RMEM a b
                    case 15:
                        {
                            ushort A = Argument();
                            ushort B = Argument();

                            Registers[GetRegister(A)] = Memory[GetValue(B)];
                        }
                        break;
                    
                    //WMEM a b
                    case 16:
                        {
                            ushort A = Argument();
                            ushort B = Argument();

                            Memory[GetValue(A)] = GetValue(B);
                        }
                        break;
                    
                    //CALL a
                    case 17:
                        {
                            ushort A = Argument();

                            Stack.Push(PC);
                            PC = GetValue(A);
                        }
                        break;
                    
                    //RET 
                    case 18:
                        {
                            PC = Stack.Pop(); 
                        }
                        break;

                    //OUT a
                    case 19:
                        {
                            ushort A = Argument();

                            Console.Write((char)GetValue(A));
                        }
                        break;

                    //IN a
                    case 20:
                        {
                            ushort A = Argument();
                       
                            char Key = Console.ReadKey().KeyChar;
                            if (Key == '\r') Key = '\n';
                            Registers[GetRegister(A)] = Key;
                            
                        }
                        break;

                    //NOOP
                    case 21:
                        break;

                    default:
                        break;
                }
            }
        }

        private ushort Argument()
        {
            PC++;
            return Memory[PC - 1];
        }

        private ushort GetValue(ushort Argument)
        {
            if (Argument < 32768)
            {
                return Argument;
            }
            else if (Argument > 32775)
            {
                throw new Exception("Invalid argument.");
            }
            else
            {
                return Registers[Argument - 32768];
            }
        }

        private ushort GetRegister(ushort Argument)
        {
            return (ushort)(Argument - 32768);
        }
    }
}
