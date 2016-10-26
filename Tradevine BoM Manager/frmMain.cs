using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Tradevine_BoM_Manager
{
    public partial class frmMain : Form
    {
        // Master list of products
        List<Product> products;

        // Current file that we are working on
        string currentFile = "";

        public frmMain()
        {
            InitializeComponent();

            ResetForm();
        }

        private void ResetForm()
        {
            // Clear the listboxes
            lbxBoM.Items.Clear();
            lbxProducts.Items.Clear();

            // Disable form elements
            lbxProducts.Enabled = false;
            lbxBoM.Enabled = false;
            nudQuantity.Enabled = false;
            tbxAddBoMItem.Enabled = false;
            btnAddBoMItem.Enabled = false;
            mnuFileSave.Enabled = false;
            mnuFileSaveAs.Enabled = false;

            // Reset default values
            nudQuantity.Value = 0;
            tbxAddBoMItem.Text = "";
        }

        /// <summary>
        /// Loads a TradeVine BoM CSV into a list of Products
        /// </summary>
        /// <param name="fn">A string containing the filename of the CSV to load</param>
        /// <returns>A list of products extracted from the CSV file</returns>
        private List<Product> LoadCSV(string fn)
        {
            // Create a temporary list of products
            List<Product> newProducts = new List<Product>();

            // Try to open the CSV
            try
            {
                // Open the file with a StreamReader
                StreamReader sr = new StreamReader(fn);

                // Loop through the file one line at a time
                while (!sr.EndOfStream)
                {
                    // Read the line of text as a record
                    string record = sr.ReadLine();

                    // Split the record into cells at the ,
                    string[] recordCells = record.Split(',');

                    if (recordCells.Count() != 4)
                    {
                        MessageBox.Show("CSV File has the incorrect number of collumns. Please make sure the file has the following collumns:\n\nBoM Product Code\nBoM Component Product Code\nBoM Component Quantity\nBoM Remove", "Invalid format");
                        break;
                    }

                    // Ignore the header
                    if (recordCells[0] != "BoM Product Code")
                    {
                        // Create a new BoMItem from the cells in the record
                        BoMItem bomItem = new BoMItem(recordCells[1], (recordCells[3] == "Yes") ? -1 : Convert.ToInt32(recordCells[2]));

                        // Placeholder variable that determines whether this BoMItem was added to an
                        // existing product or needs to be added to a new one
                        bool addToNew = true;

                        // Loop through all of the products to see if the product this BoM item belongs
                        // to already exists
                        foreach (Product p in newProducts)
                        {
                            // Check the ProductCode with the first cell in the record
                            if (p.ProductCode == recordCells[0])
                            {
                                // If the product this BoMItem belongs to already exists, then add the
                                // BoMItem to that product
                                p.BomItems.Add(bomItem);

                                // Indicate that we don't need to add this BoMItem to a new product
                                addToNew = false;

                                // Break the foreach loop as there should be only one of each product
                                // in the list of new products
                                break;
                            }
                        }

                        // If the BoMItem needs to be added to a new product
                        if (addToNew)
                        {
                            // Create the product
                            Product newProduct = new Product(recordCells[0], new List<BoMItem>());

                            // Add the BoMItem to the new product
                            newProduct.BomItems.Add(bomItem);

                            // Add the new product to the newProducts list
                            newProducts.Add(newProduct);
                        }
                    }
                }

                sr.Close();

                currentFile = fn;
            }
            catch
            {
                MessageBox.Show("There was an error opening the file. Please make sure you have permission to access the file, and that the file is not already in use by another program");
            }

            // Return the list of new products
            return newProducts;
        }

        /// <summary>
        /// Saves a list of Products into a TradeVine BoM CSV
        /// </summary>
        /// <param name="fn">A string containing the filename of the CSV to load</param>
        /// <param name="pl">The list of Products to save</param>
        private void SaveCSV(string fn, List<Product> pl)
        {

            // Try to save the CSV
            try
            {
                // Save the file with a StreamWriter
                StreamWriter sw = new StreamWriter(fn);
                sw.WriteLine("BoM Product Code,BoM Component Product Code,BoM Component Quantity,BoM Remove");
                foreach (Product p in pl)
                {
                    foreach (BoMItem b in p.BomItems)
                    {
                        sw.WriteLine(p.ProductCode + "," + b.ProductCode + "," + Math.Max(0, b.Quantity) + "," + ((b.Quantity == -1) ? "Yes" : "No"));
                    }
                }

                sw.Close();

                currentFile = fn;
            }
            catch
            {
                MessageBox.Show("There was an error saving the file. Please make sure you have permission to access the file, and that the file is not already in use by another program");
            }
        }

        /// <summary>
        /// Open a TradeVine BoM CSV and load its contents into the form
        /// </summary>
        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            // Create an instance of the open file dialog box.
            OpenFileDialog ofd = new OpenFileDialog();

            // Set filter options and filter index.
            ofd.Filter = "CSV Files (.csv)|*.csv|All Files (*.*)|*.*";
            ofd.FilterIndex = 1;

            // Process input if the user clicked OK.
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Load the CSV into the master product list
                products = LoadCSV(ofd.FileName);

                // Remove previous data from the form
                ResetForm();

                // Try adding the new products to the form
                try
                {
                    foreach (Product p in products)
                    {
                        lbxProducts.Items.Add(p);
                    }

                    // Enable the products listbox and save buttons
                    lbxProducts.Enabled = true;
                    mnuFileSave.Enabled = true;
                    mnuFileSaveAs.Enabled = true;
                }
                catch
                {
                    MessageBox.Show("There was a problem loading the data, please try again");
                }
            }
        }

        /// <summary>
        /// Save the changes to the file that was opened
        /// </summary>
        private void mnuFileSave_Click(object sender, EventArgs e)
        {
            // Save the master product list into the CSV file we opened earlier
            SaveCSV(currentFile, products);
        }

        /// <summary>
        /// Save the changes to a new file
        /// </summary>
        private void mnuFileSaveAs_Click(object sender, EventArgs e)
        {
            // Create an instance of the save file dialog box.
            SaveFileDialog sfd = new SaveFileDialog();

            // Set filter options and filter index.
            sfd.Filter = "CSV Files (.csv)|*.csv|All Files (*.*)|*.*";
            sfd.FilterIndex = 1;

            // Process input if the user clicked OK.
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                // Save the master product list into the CSV file
                SaveCSV(sfd.FileName, products);
            }
        }

        /// <summary>
        /// Close the program
        /// </summary>
        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Show the BoMItems that the selected Products have in common in the BoM listbox
        /// </summary>
        private void lbxProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear the BoM listbox in preperation for new data
            lbxBoM.Items.Clear();
            lbxBoM.Enabled = false;
            nudQuantity.Enabled = false;
            nudQuantity.Value = 0;
            btnAddBoMItem.Enabled = false;

            // Check to make sure that there is at least one product selected
            if (lbxProducts.SelectedItems.Count > 0)
            {
                // Get a list of the selected products
                List<Product> selectedProducts = lbxProducts.SelectedItems.Cast<Product>().ToList();

                // Create a list of BoMItems that the products have in common.
                // We start with theBoMItems of the first product in the selected products list
                List<BoMItem> commonBoMItems = new List<BoMItem>(selectedProducts.First().BomItems);

                // Loop through each selected Product
                foreach (Product selectedProduct in selectedProducts)
                {
                    // Loop through each item in the current set of common BoMItems
                    foreach (BoMItem commonBoMItem in selectedProducts.First().BomItems)
                    {
                        // If one of the currently selected products does not contain one of
                        // the items in the list of common BoMItems, or one of its items has
                        // a different quantity, it needs to be removed from the list.
                        bool remove = true;

                        // If one of the currently selected products does not contain the same
                        // quantity for one of the items in the list of common BoMItems, it
                        // needs to be disabled.

                        // Loop through the BoMItems of each selected object and compare them
                        // with each item in the list of common BoMItems
                        foreach (BoMItem selectedProductBoM in selectedProduct.BomItems)
                        {
                            // If there is a match, remove the flag to be removed
                            if (commonBoMItem.ProductCode == selectedProductBoM.ProductCode && commonBoMItem.Quantity == selectedProductBoM.Quantity)
                            {
                                remove = false;
                            }
                        }

                        // If there was no match, remove the BoMItem as it doesn't belong with
                        // all of the selected products
                        if (remove)
                        {
                            commonBoMItems.Remove(commonBoMItem);
                        }
                    }
                }

                // Add the list of common BoMItems to the BoM listbox
                foreach (BoMItem item in commonBoMItems)
                {
                    lbxBoM.Items.Add(item);
                }

                // Enable the listbox
                lbxBoM.Enabled = true;
                tbxAddBoMItem.Text = "";
                tbxAddBoMItem.Enabled = true;
                btnAddBoMItem.Enabled = true;
            }
        }

        /// <summary>
        /// Enable/Disable the nudQunatity control and fill it with info of the selected BoMItem
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lbxBoM_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Only enable nudQuantity if there is an item selected
            if (lbxBoM.SelectedItems.Count > 0)
            {
                nudQuantity.Value = lbxBoM.SelectedItems.Cast<BoMItem>().ToList().First().Quantity;
                nudQuantity.Enabled = true;
            }
            else
            {
                nudQuantity.Enabled = false;
                nudQuantity.Value = 0;
            }
        }


        /// <summary>
        /// Change the quantity of the selected products BoMItems
        /// </summary>
        private void nudQuantity_ValueChanged(object sender, EventArgs e)
        {
            if (nudQuantity.Enabled == true)
            {
                // Get a list of the selected products
                List<Product> selectedProducts = lbxProducts.SelectedItems.Cast<Product>().ToList();

                // Loop through each selected Product
                foreach (Product selectedProduct in selectedProducts)
                {
                    // Loop through each BoMItem in each selected product
                    foreach (BoMItem bomItem in selectedProduct.BomItems)
                    {
                        // If the BoMItem matches the one selected in the BoM listbox then change its quantity
                        if (bomItem.ProductCode == lbxBoM.SelectedItems.Cast<BoMItem>().ToList().First().ProductCode)
                        {
                            bomItem.Quantity = Convert.ToInt32(nudQuantity.Value);
                        }
                    }
                }
            }
        }

        private void btnAddBoMItem_Click(object sender, EventArgs e)
        {
            if (tbxAddBoMItem.Text != "")
            {
                // Create a new BoMItem
                BoMItem newBoMItem = new BoMItem(tbxAddBoMItem.Text, 1);

                // Get a list of the selected products
                List<Product> selectedProducts = lbxProducts.SelectedItems.Cast<Product>().ToList();

                // Loop through each selected Product
                foreach (Product selectedProduct in selectedProducts)
                {
                    bool added = false;

                    // Loop through each BoMItem in each selected product
                    foreach (BoMItem bomItem in selectedProduct.BomItems)
                    {
                        // If the new BoMItem matches any of the ones from the selected products
                        // then increment it instead of adding it
                        if (bomItem.ProductCode == newBoMItem.ProductCode)
                        {
                            bomItem.Quantity = Math.Max(0, bomItem.Quantity) + 1;
                            added = true;
                        }
                    }

                    // If the selected product doesn't already have the BoMItem, then add it
                    if (!added)
                    {
                        selectedProduct.BomItems.Add(newBoMItem);
                    }
                }
            }
        }
    }
}