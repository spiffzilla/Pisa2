using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
namespace PisaNamespace
{
    public partial class Pisa2 : Form
    {
        double raiseCurrentYear = Properties.Settings.Default.raise_current_year;
        double raiseNextYear = Properties.Settings.Default.raise_next_year;
        double euroRateCurrentYear = Properties.Settings.Default.euro_rate_current;
        double euroRateNextYear = Properties.Settings.Default.euro_rate_next_year;
        string currentYear = Properties.Settings.Default.current_year;
        string nextYear = Properties.Settings.Default.next_year;
        ArticleFactorCollection collection = new ArticleFactorCollection();
        DateTime today = DateTime.Today;

        public Pisa2()
        {
            InitializeComponent();
            InitializeVar();
            InitializeGUI();
            InitializeConfig();
            /*InitializeGridview();*/
            this.dataGridViewFactors.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewFactors_DataError);

            populateDropdown();
            
        }

        public void populateDropdown()
        {
            this.factorsTableAdapter1.Fill(this.pferdDataSet01.factors);
#region Old code to fill drop down
            /*try
            {
                string[] array2 = new string[2];
                Dictionary<string, string> test = new Dictionary<string, string>();
                int counter = 0;
                do
                {

                    array2 = collection.ReadDataGridWithArticleFactors(dgvFactors, counter);
                    // Bind combobox to dictionary
                    if (!test.Keys.Contains(array2[0]))
                    {
                        test.Add(array2[0], array2[1]);
                    }
                    counter += 1;
                } while (dgvFactors.Rows.Count - 1 > counter);
                comboBox1.DataSource = new BindingSource(test, null);
                comboBox1.DisplayMember = "Key";
                comboBox1.ValueMember = "Value";

                // Get combobox selection (in handler)
                string value = ((KeyValuePair<string, string>)comboBox1.SelectedItem).Value;


            }
            catch (Exception ex)
            {

                Console.Write(ex.ToString());
            }
            */
#endregion
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            calculateAndShow();
        }

        public int Year()
        {
            DateTime dt = DateTime.Now;
            return (dt.Year);
        }
        public double rundaAv(double value)
        {
            if (value >= 100)
            {
                // Mer än hundra, runda av till hela kronor
                return(Math.Round(value, 0));
            }
            else
            {
                // Runda av till 10öringar
                //return (Math.Round(value * 2.0d) / 2.0d);
                return (Math.Round(value, 1));
            }
        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Exit application
            System.Windows.Forms.Application.Exit();
        }
        
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutPisa2 box = new aboutPisa2();
            box.ShowDialog();
        }

        private void hjälpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormHelp frmHelp = new FormHelp();
            frmHelp.Show();
        }
        
        private void txtPisaPris_KeyDown(object sender, KeyEventArgs e)
        {
            ColorPisaPris();
            if (e.KeyCode == Keys.Enter)
            {
                if(ValidNumericInput(txtPisaPris.Text, false))
                    calculateAndShow();
            }
        }

        private void txtRabatt_KeyDown(object sender, KeyEventArgs e)
        {
            ColorRabatt();
            if (e.KeyCode == Keys.Enter)
            {
                if (ValidNumericInput(txtRabatt.Text, false))
                    calculateAndShow();
            }
        }

        private void calculateAndShow()
        {
            double pisaPrice = 0;
            double factor = 0;
            double rabatt = 0;
            bool pisaCheck = false;
            bool faktorCheck = false;
            bool rabattCheck = false;
            bool show12 = true;

            if (string.IsNullOrEmpty(txtFaktor.Text))
            {
                txtFaktor.Text = Convert.ToString(comboBox1.SelectedValue);
            }

            if ((double.TryParse(txtPisaPris.Text, out pisaPrice)))
                pisaCheck = true;
            if ((double.TryParse(txtFaktor.Text, out factor)))
                faktorCheck = true;
            if ((double.TryParse(txtRabatt.Text, out rabatt)))
                rabattCheck = true;
            else
            {
                if (txtRabatt.Text == "" || rabatt == 0)
                {
                    rabattCheck = true;
                    show12 = false;
                }

            }
            if (pisaCheck && faktorCheck && rabattCheck)
            {
                FactorRebateOK(pisaPrice, factor, rabatt, show12);
            }
            else
            {
                StatusStripText("Kontrollera inmatade värden, Pisa pris och faktor måste vara angivna.");
                //InitializeGUI();
            }
        }

        private void FactorRebateOK(double pisaPrice, double factor, double rabatt, bool show12)
        {
            // Om allt OK

            // Variabler
            double gross = 0;
            double sliding = 0;
            double grossSekCurrentYear = 0;
            double grossEuroCurrentYear = 0;
            double grossSekNextYear = 0;
            double grossEuroNextYear = 0;

            double raiseCurrentYearConverted = 1 + (raiseCurrentYear / 100);           // Påslag med procent 2014
            double raiseNextYearConverted = 1 + (raiseNextYear / 100);           // Påslag med procent 2015

            // Räkna ut
            sliding = pisaPrice - (rabatt / 100 * pisaPrice);            // 2

            // Check if year is 2014 or later
            if (Year() == 2014)
            {
                gross = sliding / 100 * euroRateCurrentYear;                 // 1
            }
            else
            {
                gross = sliding / 100 * euroRateNextYear;                 // 1
            }
            grossSekCurrentYear = pisaPrice / 100 * factor * raiseCurrentYearConverted;  // 3
            grossEuroCurrentYear = grossSekCurrentYear / euroRateCurrentYear;              // 4
            grossSekNextYear = grossSekCurrentYear * raiseNextYearConverted;  // 3 2014
            grossEuroNextYear = grossSekNextYear / euroRateNextYear;    // 4 2015
            // Sätt uträkningarna i textboxarna
            if (show12)
            {
                txtGlidandePris.Text = gross.ToString("N").PadLeft(7);                     // 1   
                txtNettoEuro100.Text = sliding.ToString("N").PadLeft(7);                   // 2   
            }
            txtBrutto2014Sek.Text = rundaAv(grossSekCurrentYear).ToString("N").PadLeft(7);             // 3 2014  
            txtBrutto2014Euro.Text = grossEuroCurrentYear.ToString("N").PadLeft(7);           // 4 2014  
            txtBrutto2015Sek.Text = rundaAv(grossSekNextYear).ToString("N").PadLeft(7);             // 3 2015  
            txtBrutto2015Euro.Text = grossEuroNextYear.ToString("N").PadLeft(7);           // 4 2015  
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            InitializeGUI();
        }

        private void chkNewYear_CheckedChanged(object sender, EventArgs e)
        {
            CheckCheckBoxCurrent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.raise_current_year = 1.1;
            Properties.Settings.Default.raise_next_year = 2.2;
            Properties.Settings.Default.euro_rate_current = 3.3;
            Properties.Settings.Default.euro_rate_next_year = 4.4;
            Properties.Settings.Default.current_year = "1998";
            Properties.Settings.Default.next_year = "1999";
            Properties.Settings.Default.Save();
            InitializeVar();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InitializeConfig();
            InitializeVar();
            InitializeGUI();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            // Save values
            double confRaiseCurrentYearConfig = 0;
            double confRaiseNextYearConfig = 0;
            double confEuroRateCurrentYearConfig = 0;
            double confEuroRateNextYearConfig = 0;
            if (
            (Double.TryParse(txtRaiseCurrentYearConfig.Text, out confRaiseCurrentYearConfig)) &&
            (Double.TryParse(txtRaiseNextYearConfig.Text, out confRaiseNextYearConfig)) &&
            (Double.TryParse(txtEuroRateCurrentYearConfig.Text, out confEuroRateCurrentYearConfig)) &&
            (Double.TryParse(txtEuroRateNextYearConfig.Text, out confEuroRateNextYearConfig))
            )
            {
                Properties.Settings.Default.raise_current_year = confRaiseCurrentYearConfig;
                Properties.Settings.Default.raise_next_year = confRaiseNextYearConfig;
                Properties.Settings.Default.euro_rate_current = confEuroRateCurrentYearConfig;
                Properties.Settings.Default.euro_rate_next_year = confEuroRateNextYearConfig;
                Properties.Settings.Default.current_year = txtCurrentYearConfig.Text;
                Properties.Settings.Default.next_year = txtNextYearConfig.Text;
                Properties.Settings.Default.Save();
                InitializeVar();
                InitializeGUI();
                StatusStripText("Värden sparade.");
            }
            else
            {
                StatusStripText("Kontrollera inmatade värden.");
            }
        }

        public void StatusStripText(string textToStrip)
        {
            var toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripStatusLabel.Text = textToStrip;
            statusStrip.Items.Clear();
            statusStrip.Items.Add(toolStripStatusLabel);

            System.Timers.Timer timerStatusStrip = new System.Timers.Timer();
            timerStatusStrip.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            timerStatusStrip.Interval = 5000;
            timerStatusStrip.Enabled = true;

        }

        private void ClearStatusStripText()
        {
            var toolStripStatusLabel = new ToolStripStatusLabel();
            //toolStripStatusLabel.Text = textToStrip;
            statusStrip.Items.Clear();
            //statusStrip.Items.Add(toolStripStatusLabel);
        }

        private void Pisa2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            double pisaPrice = 0;
            if (comboBox1.SelectedValue != null)
            {
                txtFaktor.Text = comboBox1.SelectedValue.ToString();
                if (double.TryParse(txtPisaPris.Text, out pisaPrice))
                    calculateAndShow();
            }
        }

        
/*        public void InitializeGridview()
        {
            collection.FillDataGridWithArticleFactors(dgvFactors);
        }*/

        public void InitializeConfig()
        {
            txtRaiseCurrentYearConfig.Text = Properties.Settings.Default.raise_current_year.ToString();
            txtRaiseNextYearConfig.Text = Properties.Settings.Default.raise_next_year.ToString();
            txtEuroRateCurrentYearConfig.Text = Properties.Settings.Default.euro_rate_current.ToString();
            txtEuroRateNextYearConfig.Text = Properties.Settings.Default.euro_rate_next_year.ToString();
            txtCurrentYearConfig.Text = Properties.Settings.Default.current_year.ToString();
            txtNextYearConfig.Text = Properties.Settings.Default.next_year.ToString();
        }

        public void InitializeVar()
        {
            raiseCurrentYear = Properties.Settings.Default.raise_current_year;
            raiseNextYear = Properties.Settings.Default.raise_next_year;
            euroRateCurrentYear = Properties.Settings.Default.euro_rate_current;
            euroRateNextYear = Properties.Settings.Default.euro_rate_next_year;
            currentYear = Properties.Settings.Default.current_year;
            nextYear = Properties.Settings.Default.next_year;
            DateTime today = DateTime.Today;
        }

        public void InitializeGUI()
        {
            lblHojning2014.Text = raiseCurrentYear.ToString() + " %";
            lblHojning2015.Text = raiseNextYear.ToString() + " %";
            lblEurokurs2014.Text = euroRateCurrentYear.ToString() + " Kr";
            lblEurokurs2015.Text = euroRateNextYear.ToString() + " Kr";
            txtGlidandePris.Text = "   ---";
            txtNettoEuro100.Text = "   ---";
            txtBrutto2014Sek.Text = "   ---";
            txtBrutto2014Euro.Text = "   ---";
            txtBrutto2015Sek.Text = "   ---";
            txtBrutto2015Euro.Text = "   ---";
            txtFaktor.Text = "";
            txtPisaPris.Text = "";
            txtRabatt.Text = "";
            lblNextYearSek.Text = currentYear;
            lblNextYearEuro.Text = currentYear;
            lblBruttoSekNextYear.Text = "Bruttopris " + currentYear + " SEK";
            lblBruttoEuroNextYear.Text = "Bruttopris " + currentYear + " Euro";
            lblRaiseNextYear.Text = "Höjning " + currentYear;
            lblEuroRateNextYear.Text = "Eurokurs " + currentYear;
            lblCurrentYearSek.Text = currentYear.ToString();
            lblCurrentYearEuro.Text = currentYear.ToString();
            lblNextYearSek.Text = nextYear.ToString();
            lblNextYearEuro.Text = nextYear.ToString();
            lblBruttoSekCurrentYear.Text = "Bruttopris " + currentYear + " SEK";
            lblBruttoEuroCurrentYear.Text = "Bruttopris " + currentYear + " Euro";
            lblBruttoSekNextYear.Text = "Bruttopris " + nextYear + " SEK";
            lblBruttoEuroNextYear.Text = "Bruttopris " + nextYear + " Euro";
            lblRaiseCurrentYear.Text = "Höjning " + currentYear;
            lblRaiseNextYear.Text = "Höjning " + nextYear;
            lblEuroRateCurrentYear.Text = "Eurokurs " + currentYear;
            lblEuroRateNextYear.Text = "Eurokurs " + nextYear;
            CheckCheckBoxCurrent();
            tabPage1.Text = @"Räkna";
            tabPage2.Text = @"Konfigurera";
            tabPage3.Text = @"Faktorer";
            btnCalculate.Enabled = false;
            ClearStatusStripText();
        }
        private void CheckCheckBoxCurrent()
        {
            if (!chkCurrentYear.Checked)
            {
                HideLabels();
            }
            else
            {
                ShowLabels();
            }
        }

        private void ShowLabels()
        {
            lblBruttoSekNextYear.Visible = true;
            lblBruttoEuroNextYear.Visible = true;
            txtBrutto2015Sek.Visible = true;
            txtBrutto2015Euro.Visible = true;
            label18.Visible = true;
            label19.Visible = true;
            lblNextYearSek.Visible = true;
            lblNextYearEuro.Visible = true;
            lblRaiseNextYear.Visible = true;
            lblEuroRateNextYear.Visible = true;
            lblHojning2015.Visible = true;
            lblEurokurs2015.Visible = true;
        }

        private void HideLabels()
        {
            lblBruttoSekNextYear.Visible = false;
            lblBruttoEuroNextYear.Visible = false;
            txtBrutto2015Sek.Visible = false;
            txtBrutto2015Euro.Visible = false;
            label18.Visible = false;
            label19.Visible = false;
            lblNextYearSek.Visible = false;
            lblNextYearEuro.Visible = false;
            lblRaiseNextYear.Visible = false;
            lblEuroRateNextYear.Visible = false;
            lblHojning2015.Visible = false;
            lblEurokurs2015.Visible = false;
        }



        ////////////////////////////////////////////////////
        private void dataGridViewFactors_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }

        private void btnSaveArticleFactor_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                this.factorsBindingSource2.EndEdit();
                this.factorsTableAdapter.Update(this.dataSetPferd.factors);

                /*
                this.Validate();
                this.factorsBindingSource1.EndEdit();
                this.factorsTableAdapter1.Update(this.pferdDataSet01.factors);
                */
                StatusStripText("Informationen sparades.");
                // Reload drop down with updated values
                //populateDropdown();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(string.Format("Det gick inte att spara: {0}", ex.ToString()));
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell oneCell in dataGridViewFactors.SelectedCells)
            {
                if (oneCell.Selected)
                    dataGridViewFactors.Rows.RemoveAt(oneCell.RowIndex);
            }
            StatusStripText("Raden/raderna raderades.");
        }

        private void txtPisaPris_Validating(object sender, CancelEventArgs e)
        {
            //ValidNumericInput(txtPisaPris.Text);
        }

        private bool ValidNumericInput(string textToValidate, bool displayErrorDialog = true)
        {
            bool result = true;
            var d = 0.0;
            
            if ((!string.IsNullOrEmpty(textToValidate)) && (!Double.TryParse(textToValidate, out d)))
            {
                result = false;
                if (displayErrorDialog == true)
                {
                    StatusStripText("Ej numeriskt värde - vänligen kontrollera inmatningen.");
                }
            }
            
            btnCalculate.Enabled = result;
            return result;
        }

        private void txtPisaPris_KeyPress(object sender, KeyPressEventArgs e)
        {
            //
        }

        private void ColorPisaPris()
        {
            if (!ValidNumericInput(txtPisaPris.Text, false))
            {
                txtPisaPris.BackColor = Color.Crimson;
                txtPisaPris.ForeColor = Color.White;
            }
            else
            {
                txtPisaPris.BackColor = Color.White;
                txtPisaPris.ForeColor = Color.Black;
            }
        }

        private void ColorRabatt()
        {
            if (!ValidNumericInput(txtRabatt.Text, false))
            {
                txtRabatt.BackColor = Color.Crimson;
                txtRabatt.ForeColor = Color.White;
            }
            else
            {
                txtRabatt.BackColor = Color.White;
                txtRabatt.ForeColor = Color.Black;
            }
        }

        
        private void txtPisaPris_KeyUp(object sender, KeyEventArgs e)
        {
            ColorPisaPris();
        }

        private void txtRabatt_KeyUp(object sender, KeyEventArgs e)
        {
            ColorRabatt();
        }

        private void Pisa2_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSetPferd.factors' table. You can move, or remove it, as needed.
            this.factorsTableAdapter.Fill(this.dataSetPferd.factors);
            // TODO: This line of code loads data into the 'pferdDataSet01.factors' table. You can move, or remove it, as needed.
            this.factorsTableAdapter1.Fill(this.pferdDataSet01.factors);
            /*
            comboBox1.SelectedIndex = 1;
            comboBox1.Select();
            comboBox1.Focus();*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ClearStatusStripText();
        }

        void OnTimedEvent(object sender, EventArgs e)
        //private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            ClearStatusStripText();
            timerStatusStrip.Enabled = false;
        }

        private void dataGridViewFactors_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // After leaving datagridview cell
            return;

            // DEBUG - not executed
            try
            {
                string test = dataGridViewFactors.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                var results = from myRow in this.pferdDataSet01.Tables[0].AsEnumerable() where myRow.Field<string>("key") == test select myRow;
                if (results.Count() > 0)
                {
                    MessageBox.Show("Aja baja");
                    dataGridViewFactors.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                    /*dataGridView1.ClearSelection();
                    dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    dataGridView1.Rows[e.RowIndex].Selected = true;*/
                    factorsBindingSource1.MoveNext();
                    factorsBindingSource1.MovePrevious();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Debug" + ex.ToString());
                throw;
            }
        }

        // An error in the data occurred.
        private void dataGridViewFactors_DataError(object sender,
            DataGridViewDataErrorEventArgs e)
        {
            // Don't throw an exception when we're done.
            e.ThrowException = false;

            // Display an error message.
            string txt = "Error with " +
                dataGridViewFactors.Columns[e.ColumnIndex].HeaderText +
                "\n\n" + e.Exception.Message;

            MessageBox.Show("Inmatade värde(n) är inte OK, gör om gör rätt.\n\n"+e.Exception.Message, "Fel inmatning",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            StatusStripText("Felaktig inmatning, vänligen kontrollera inmatningen.");

            // If this is true, then the user is trapped in this cell.
            e.Cancel = false;
            foreach (DataGridViewCell oneCell in dataGridViewFactors.SelectedCells)
            {
                if (oneCell.Selected)
                    dataGridViewFactors.Rows.RemoveAt(oneCell.RowIndex);
            }
        }
    }
}
