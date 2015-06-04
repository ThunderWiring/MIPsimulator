using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MIPS
{
    public partial class MIPS_Demo : Form
    {
        #region Variables
        byte[] registerFile = new byte[32];
        List<byte> data_memory = new List<byte>();
        List<int> instruction_memory = new List<int>();
        Compile compiler = new Compile();
        string instruction;
        private int programe_counter = 0, error_counter = 0;
        #endregion//Variables

        public MIPS_Demo()
        {
            InitializeComponent();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();            
        }
        private void reset_reg_file()
        {
            for (int i = 0; i < registerFile.Length; i++)
            {
                registerFile[i] = 0;
            }
            update_regFile();
        }
        private void resetRegFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset_reg_file();
        }
        /// <summary>
        /// updating the register file in the Output tab page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButton_run_Click(object sender, EventArgs e)
        {
            instruction_memory.Clear();
            reset_reg_file();
            compiler.dataMem = this.data_memory;
            
            build_code();
            
            if (compiler.erros > 0)
            {
                MessageBox.Show("error found","",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            compile_code();            
            
            registerFile = compiler.registerFile;
            update_regFile();
            update_data_memory();
        }

        private void update_data_memory()
        {
            data_memory = compiler.dataMem;
            richTextBox_dataMem.Clear();
            byte[] temp_data_mem;
            temp_data_mem = data_memory.ToArray();
            for (int i = 0; i < temp_data_mem.Length; i++)
            {
                string tmp = Convert.ToString(data_memory[i], 16).PadLeft(8,'0');
                tmp += '\n';
                richTextBox_dataMem.AppendText(tmp);
            }
        }

        /// <summary>
        /// updates the register values in the output tab page.
        /// </summary>
        private void update_regFile()
        {
            label_r0.Text = Convert.ToString(registerFile[0], 2).PadLeft(8, '0');
            label_r1.Text = Convert.ToString(registerFile[1], 2).PadLeft(8, '0');
            label_r2.Text = Convert.ToString(registerFile[2], 2).PadLeft(8, '0');
            label_r3.Text = Convert.ToString(registerFile[3], 2).PadLeft(8, '0');
            label_r4.Text = Convert.ToString(registerFile[4], 2).PadLeft(8, '0');
            label_r5.Text = Convert.ToString(registerFile[5], 2).PadLeft(8, '0');
            label_r6.Text = Convert.ToString(registerFile[6], 2).PadLeft(8, '0');
            label_r7.Text = Convert.ToString(registerFile[7], 2).PadLeft(8, '0');
            label_r8.Text = Convert.ToString(registerFile[8], 2).PadLeft(8, '0');
            label_r9.Text = Convert.ToString(registerFile[9], 2).PadLeft(8, '0');
            label_r10.Text = Convert.ToString(registerFile[10], 2).PadLeft(8, '0');
            label_r11.Text = Convert.ToString(registerFile[11], 2).PadLeft(8, '0');
            label_r12.Text = Convert.ToString(registerFile[12], 2).PadLeft(8, '0');
            label_r13.Text = Convert.ToString(registerFile[13], 2).PadLeft(8, '0');
            label_r14.Text = Convert.ToString(registerFile[14], 2).PadLeft(8, '0');
            label_r15.Text = Convert.ToString(registerFile[15], 2).PadLeft(8, '0');
            label_r16.Text = Convert.ToString(registerFile[16], 2).PadLeft(8, '0');
            label_r17.Text = Convert.ToString(registerFile[17], 2).PadLeft(8, '0');
            label_r18.Text = Convert.ToString(registerFile[18], 2).PadLeft(8, '0');
            label_r19.Text = Convert.ToString(registerFile[19], 2).PadLeft(8, '0');
            label_r20.Text = Convert.ToString(registerFile[20], 2).PadLeft(8, '0');
            label_r21.Text = Convert.ToString(registerFile[21], 2).PadLeft(8, '0');
            label_r22.Text = Convert.ToString(registerFile[22], 2).PadLeft(8, '0');
            label_r23.Text = Convert.ToString(registerFile[23], 2).PadLeft(8, '0');
            label_r24.Text = Convert.ToString(registerFile[24], 2).PadLeft(8, '0');
            label_r25.Text = Convert.ToString(registerFile[25], 2).PadLeft(8, '0');
            label_r26.Text = Convert.ToString(registerFile[26], 2).PadLeft(8, '0');
            label_r27.Text = Convert.ToString(registerFile[27], 2).PadLeft(8, '0');
            label_r28.Text = Convert.ToString(registerFile[28], 2).PadLeft(8, '0');
            label_r29.Text = Convert.ToString(registerFile[29], 2).PadLeft(8, '0');
            label_r30.Text = Convert.ToString(registerFile[30], 2).PadLeft(8, '0');
            label_r31.Text = Convert.ToString(registerFile[31], 2).PadLeft(8, '0');
        }

        /// <summary>
        /// runs through the lines of the code in the rich texbox by activating the timer-
        /// which constantly calls the decod function in.
        /// </summary>
        private void build_code()
        {
            int counter = 0, pc =0;            
            while (counter < richTextBox_codeArena.Lines.Length)
            {
                instruction = richTextBox_codeArena.Lines[counter];
                if (instruction.Length == 0)
                {
                    counter++;
                    continue;
                }
                if (instruction[0] == '#' || instruction[0] == '\n')
                {
                    counter++;
                    continue;
                }
                compiler.convert_code(instruction);
                registerFile = compiler.registerFile;
                instruction_memory = compiler.instructionMemory;                
                richTextBox_instMem.AppendText(Convert.ToString(instruction_memory[pc], 2).PadLeft(32,'0')  + '\n');
                compiler.machineInstruction = "";

                counter++;
                pc++;                
            }
            data_memory = compiler.dataMem;            
        }

        private void compile_code()
        {
            programe_counter = compiler.programeCounter;
            compiler.registerFile = registerFile;
            string instruction = "";
            int[] tmp = instruction_memory.ToArray();
            while (programe_counter < tmp.Length)
            {
                instruction = richTextBox_instMem.Lines[programe_counter];
                compiler.CompileMachineInst(instruction);
                programe_counter = compiler.programeCounter;
            }           
        }

        private void dataMemoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string data_mem_filePath = load_file();
            richTextBox_dataMem.Text = File.ReadAllText(data_mem_filePath);
            string[] hexValuesSplit = richTextBox_dataMem.Text.Split('\n', '\r');
            //adding the data memory to the list:
            for (int i = 0; i < hexValuesSplit.Length; i+=2)
            {
                int value = Convert.ToInt32(hexValuesSplit[i], 16);
                data_memory.Add((byte)value);
            }
            compiler.dataMem = data_memory;
        }       
        private string load_file()
        {
            string filePath = "";
            OpenFileDialog file_dialog = new OpenFileDialog();
            file_dialog.Filter = "Desktop|*.txt";
            System.Windows.Forms.DialogResult dialog_result = file_dialog.ShowDialog();
            if (dialog_result == DialogResult.OK)
            {
                filePath = file_dialog.FileName;
            }
            return filePath;
        }

        private void toolStripButton_emptyInstMemTxt_Click(object sender, EventArgs e)
        {
            //data_memory.Clear();
            instruction_memory.Clear();
            richTextBox_instMem.Clear();
            compiler.machineInstruction = "";
            compiler.instructionMemory.Clear();
            instruction_memory.Clear();
            reset_reg_file();
        }

        private void toolStripButton_addInst_Click(object sender, EventArgs e)
        {
            Add_pseudo_inst add_inst_form = new Add_pseudo_inst();
            add_inst_form.Show();
        }
   
    }
}
