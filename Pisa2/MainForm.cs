﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            
            populateDropdown();
            
        }

        public void populateDropdown()
        {
            this.factorsTableAdapter.Fill(this.pferdDataSet.factors);
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
#region Catching keys and exit etc.
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
            if (e.KeyCode == Keys.Enter)
            {
                calculateAndShow();
            }
        }
        private void txtFaktor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                calculateAndShow();
            }
        }
        private void txtRabatt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                calculateAndShow();
            }
        }
#endregion
        private void calculateAndShow()
        {
            double pisaPrice = 0;
            double factor = 0;
            double rabatt = 0;
            bool pisaCheck = false;
            bool faktorCheck = false;
            bool rabattCheck = false;
            bool show12 = true;

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
                    gross = sliding / 100 * euroRateCurrentYear;                 // 1
                else
                    gross = sliding / 100 * euroRateNextYear;                 // 1
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
            else
            {
                MessageBox.Show("Kontrollera inmatade värden, Pisa pris och faktor måste vara angivna.",
                "Fel!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
                InitializeGUI();
            }
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
                MessageBox.Show("Värden sparade.",
                "Ok",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Kontrollera inmatade värden.",
                "Fel!",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }

        private void Pisa2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            double pisaPrice = 0;
            txtFaktor.Text = comboBox1.SelectedValue.ToString();
            if (double.TryParse(txtPisaPris.Text, out pisaPrice))
                calculateAndShow();
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
        }
        private void CheckCheckBoxCurrent()
        {
            if (!chkCurrentYear.Checked)
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
            else
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
        }



        ////////////////////////////////////////////////////
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void label12_Click(object sender, EventArgs e) { }
        private void label9_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }

        private void btnSaveArticleFactor_Click(object sender, EventArgs e)
        {
            try
            {
                this.Validate();
                this.factorsBindingSource.EndEdit();
                this.factorsTableAdapter.Update(this.pferdDataSet.factors);
                MessageBox.Show("Informationen sparades");
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(string.Format("Det gick inte att spara: {0}", ex.ToString()));
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell oneCell in dataGridView1.SelectedCells)
            {
                if (oneCell.Selected)
                    dataGridView1.Rows.RemoveAt(oneCell.RowIndex);
            }
        }
    }
}
