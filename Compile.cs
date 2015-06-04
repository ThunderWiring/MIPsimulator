using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MIPS
{
    class Compile 
    {
        #region Variables


        private byte[] register_file = new byte[32];
        private List<byte> dataMemory = new List<byte>();
        private List<int> inst_mem = new List<int>();
        //instruction feilds:
        private int opCode = 0,offset = 0, rs = 0, immediate = 0, rt = 0, rd = 0, shamt = 0, function = 0;
        private int target_address = 0;
        //flags for getting machine-instr.
        private bool Rtype = false, Itype = false, Jtype = false, branch = false;
        private string machine_instruction = "";
        private int pc = 0;
        private int error_counter = 0;
        #endregion//Variables
        #region constructor
        public Compile()
        { 

        }
        #endregion//constructor

        #region Methods
        /// <summary>
        /// takes a string which represent a line in the code. identifies the instruction type
        /// and calculates the instruction fields
        /// then calls the right method to tackle this lie of code.
        /// </summary>
        /// <param name="instruciton"></param> string which represent a line in the code
        public void convert_code(string instruction)
        {            
            char[] delimiterChars = { ' ', ',', '.', ':', '\t', '\n', '$', '(', ')' };
            string[] words = instruction.Split(delimiterChars);
            //getting rs,rt,rd:
            if (words[0] == "slt" || words[0] == "add" || words[0] == "sub" ||
                words[0] == "or" || words[0] == "and" || words[0] == "xor")
            {
                opCode = 0;
                shamt = 0;
                if (int.TryParse(words[2], out rd)) { }
                if (int.TryParse(words[4], out rs)) { }
                if (int.TryParse(words[6], out rt)) { }
            }

            switch (words[0])
            {
                case "nop":
                    opCode = 0;
                    rd = rs = rt = function = shamt = 0;
                    Rtype = true;
                    get_machine_instruction();
                    break;           
                case "add":
                    function = Convert.ToInt32("100000", 2);                    
                    Rtype = true;
                    get_machine_instruction();
                    break;
                case "sub":
                    function = Convert.ToInt32("100010", 2);                    
                    Rtype = true;
                    get_machine_instruction();
                    break;
                case "or":
                    function = Convert.ToInt32("100101", 2);                    
                    Rtype = true;
                    get_machine_instruction();
                    break;
                case "and":
                    function = Convert.ToInt32("100100", 2);                   
                    Rtype = true;
                    get_machine_instruction();
                    break;
                case "xor":
                    function = Convert.ToInt32("100110", 2);                    
                    Rtype = true;
                    get_machine_instruction();
                    break;
                case "slt":
                    function = Convert.ToInt32("101010", 2);                   
                    Rtype = true;
                    get_machine_instruction();
                    break;
                case "sll":
                    function = Convert.ToInt32("000000", 2);
                    if (int.TryParse(words[2], out rt)) { }
                    if (int.TryParse(words[3], out shamt)) { }                   
                    Rtype = true;
                    get_machine_instruction();
                    break;
                case "srl":
                    function = Convert.ToInt32("000010", 2);
                    if (int.TryParse(words[2], out rt)) { }
                    if (int.TryParse(words[3], out shamt)) { }                   
                    Rtype = true;
                    get_machine_instruction();
                    break;
                case "jr":
                    function = Convert.ToInt32("001000", 2);
                    if (int.TryParse(words[3], out rs)) { }
                    Rtype = true;
                    get_machine_instruction();
                    break;
                case "addi":
                    opCode = 8;
                    if (int.TryParse(words[2], out rt)) { }
                    if (int.TryParse(words[4], out rs)) { }
                    if (int.TryParse(words[5], out immediate)) { }                   
                    Itype = true;
                    get_machine_instruction();
                    break;
                case "lw":
                    if (dataMemory == null)
                        error_counter++;
                    opCode = 35;
                    if (int.TryParse(words[5], out rs)) { }
                    if (int.TryParse(words[3], out immediate)) { }
                    if (int.TryParse(words[2], out rt)) { }
                    offset = immediate + register_file[rs];                    
                    Itype = true;
                    get_machine_instruction();
                    break;
                case "sw":
                    if (dataMemory == null)
                        error_counter++;
                    opCode = 43;
                    if (int.TryParse(words[3], out immediate)) { }
                    if (int.TryParse(words[5], out rs)) { }
                    if (int.TryParse(words[2], out rt)) { }
                    offset = immediate + register_file[rs];                  
                    Itype = true;
                    get_machine_instruction();
                    break;
                case "j":
                    opCode = 2;
                    if (int.TryParse(words[1], out target_address)) { }
                    Jtype = true;
                    get_machine_instruction();                    
                    break;
                case "beq":
                    opCode = 4;
                    if (int.TryParse(words[2], out rt)) { }
                    if (int.TryParse(words[4], out rs)) { }
                    if (int.TryParse(words[5], out immediate)) { }
                    branch = true;
                    get_machine_instruction();                    
                    break;
                case "bnq":
                    opCode = 5;
                    if (int.TryParse(words[2], out rt)) { }
                    if (int.TryParse(words[4], out rs)) { }
                    if (int.TryParse(words[5], out immediate)) { }
                    branch = true;
                    get_machine_instruction();
                    break;
            }
        }

        /// <summary>
        /// converts the instruction fields into a string in binary representation and 
        /// stratifies them into one string to represent the machine instruction
        /// </summary>
        private void get_machine_instruction()
        {
            machine_instruction += Convert.ToString(opCode, 2).PadLeft(6, '0');
            if (Rtype)
            {
                machine_instruction += Convert.ToString(rs, 2).PadLeft(5, '0');
                machine_instruction += Convert.ToString(rt, 2).PadLeft(5, '0');
                machine_instruction += Convert.ToString(rd, 2).PadLeft(5, '0');
                machine_instruction += Convert.ToString(shamt, 2).PadLeft(5, '0');
                machine_instruction += Convert.ToString(function, 2).PadLeft(6, '0');
            }
            else if (Itype)
            {
                machine_instruction += Convert.ToString(rs, 2).PadLeft(5, '0');
                machine_instruction += Convert.ToString(rt, 2).PadLeft(5, '0');
                machine_instruction += Convert.ToString(immediate, 2).PadLeft(16, '0');
            }
            else if (Jtype)
            {
                machine_instruction += Convert.ToString(target_address, 2).PadLeft(26, '0');
            }
            else if (branch)
            {
                machine_instruction += Convert.ToString(rs, 2).PadLeft(5, '0');
                machine_instruction += Convert.ToString(rt, 2).PadLeft(5, '0');
                machine_instruction += Convert.ToString(offset, 2).PadLeft(16, '0');
            }
            //add the machine instruction to the appropriate list defined in the Variables region:
            inst_mem.Add((int)(Convert.ToInt64(machine_instruction, 2)));

            machine_instruction += '\n';

            //reset instruction flags:
            Rtype = false;
            Itype = false;
            Jtype = false;
            branch = false;     
        }

        /// <summary>
        /// takes a string instruction and compile it
        /// </summary>
        /// <param name="inst"></param> the binary representation of the machine instruction set as a string
        public void CompileMachineInst(string inst)
        {
            pc = 0;
            opCode = (int)Convert.ToInt64(inst.Substring(0, 6), 2);
            if (opCode == 8 || opCode == 35 || opCode == 43 || opCode == 4 || opCode == 5)
            {
                rs = (int)Convert.ToInt64(inst.Substring(6, 5), 2);
                rt = (int)Convert.ToInt64(inst.Substring(11, 5), 2);
                immediate = (int)Convert.ToInt64(inst.Substring(16, 16), 2);
            }
            if (opCode != 2 && opCode != 3 && opCode != 4 && opCode != 5)
            {
                pc++;
            }
            switch (opCode)
            { 
                case 0:
                    #region Rtype
                    rs= (int)Convert.ToInt64(inst.Substring(6, 5), 2);                    
                    rt = (int)Convert.ToInt64(inst.Substring(11, 5), 2); 
                    rd = (int)Convert.ToInt64(inst.Substring(16, 5), 2);
                    shamt = (int)Convert.ToInt64(inst.Substring(21, 6), 2);  
                    function = (int)Convert.ToInt64(inst.Substring(26, 6), 2);

                    switch (function)
                    { 
                        case 42: //101010 :slt
                            if (register_file[rs] < register_file[rt])
                                register_file[rd] = 1;
                            else
                                register_file[rd] = 0;
                            break;
                        case 32: //100000 :add
                            register_file[rd] = (Byte)(register_file[rs] + register_file[rt]);
                            break;
                        case 34: //100010 :sub
                            register_file[rd] = (Byte)(register_file[rs] - register_file[rt]);
                            break;
                        case 37: //100101 :or
                            register_file[rd] = (Byte)(register_file[rs] | register_file[rt]);
                            break;
                        case 36: //100100 :and
                            register_file[rd] = (Byte)(register_file[rs] & register_file[rt]);
                            break;
                        case 38: //100110 :xor
                            register_file[rd] = (Byte)(register_file[rs] ^ register_file[rt]);
                            break;
                        case 00: //000000 :sll
                            register_file[rd] = (Byte)(register_file[rt] * Math.Pow(2, shamt));
                            break;
                        case 02: //000010 :srl
                            register_file[rd] = (Byte)(register_file[rt] * Math.Pow(2, -shamt));
                            break;
                    }
                    #endregion //Rtype                    
                    break;
                case 8: //addi                    
                    register_file[rt] = (Byte)(register_file[rs] + immediate);
                    break;
                case 35: //lw                    
                    offset = immediate + register_file[rs];
                    register_file[rt] = dataMemory[offset];
                    break;
                case 43: //sw
                    offset = immediate + register_file[rs];
                    dataMemory[offset] = register_file[rt];
                    break;
                case 2: //jump                    
                    pc = (int)Convert.ToInt64(inst.Substring(6, 26), 2);
                    break;
                case 4: //beq
                    if (register_file[rt] == register_file[rs])
                        pc += immediate;
                    else
                        pc++;
                    break;
                case 5: //bnq
                    if (register_file[rt] != register_file[rs])
                        pc += immediate;
                    else
                        pc++;
                    break;
            }
        }        
        #endregion //Methods
        
        #region Properties
        public int erros
        {
            get { return this.error_counter; }
            set { this.error_counter = value; }
        }
        public int programeCounter
        {
            get { return this.pc; }
            set { this.pc = value; }
        }
        public byte[] registerFile
        {
            get { return this.register_file; }
            set { this.register_file = value; }
        }
        public List<byte> dataMem
        {
            get { return this.dataMemory; }
            set { this.dataMemory = value; }
        }
        public List<int> instructionMemory
        {
            get { return this.inst_mem; }
            set { this.inst_mem = value; }
        }
        public string machineInstruction
        {
            get { return this.machine_instruction; }
            set { this.machine_instruction = value; }
        }
        #endregion //prperties
    }
}
