using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Infotols
{
    /// <summary>
    /// Logique d'interaction pour FactureWindow.xaml
    /// </summary>
    public partial class FactureWindow : Window
    {
        public FactureWindow()
        {
            InitializeComponent();
            LoadProducts();
            LoadClients();  // Charger les clients dans la ComboBox
        }

        // Charger tous les produits dans le ComboBox
        private void LoadProducts()
        {
            var products = Bdd.GetAllProduits();
            ProduitComboBox.ItemsSource = products;
            ProduitComboBox.DisplayMemberPath = "Name"; // Affiche le nom du produit
            ProduitComboBox.SelectedValuePath = "Id";   // Associe l'ID du produit à la valeur sélectionnée
        }

        // Charger tous les clients dans le ComboBox
        private void LoadClients()
        {
            var clients = Bdd.GetAllClients();  // Charger tous les clients depuis la base de données
            ClientComboBox.ItemsSource = clients;
            ClientComboBox.DisplayMemberPath = "Nom"; // Affiche le nom du client
            ClientComboBox.SelectedValuePath = "Id";  // Associe l'ID du client à la valeur sélectionnée
        }

        // Ajouter un produit à la facture
        private void AjouterProduitButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProduct = (Produit)ProduitComboBox.SelectedItem;
            if (selectedProduct == null)
            {
                MessageBox.Show("Veuillez sélectionner un produit.");
                return;
            }

            // Vérifier que la quantité est valide
            if (!int.TryParse(QuantiteTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Veuillez entrer une quantité valide.");
                return;
            }

            // Calculer le montant de la ligne (quantité * prix unitaire)
            decimal lineTotal = selectedProduct.Price * quantity;

            // Créer une nouvelle ligne de facture
            var newLine = new
            {
                Produit = selectedProduct,  // Objet produit
                Quantite = quantity,
                PrixUnitaire = selectedProduct.Price,
                Montant = lineTotal
            };

            // Ajouter la ligne au DataGrid
            ProduitsDataGrid.Items.Add(newLine);

            // Recalculer le total
            RecalculerTotal();
        }

        // Calculer le total de la facture
        private void RecalculerTotal()
        {
            decimal total = 0;
            foreach (var item in ProduitsDataGrid.Items)
            {
                dynamic row = item;
                total += row.Montant;
            }
            TotalTextBox.Text = total.ToString("F2");  // Afficher le total formaté dans la TextBox
        }

        // Enregistrer la facture dans la base de données
        private void EnregistrerFactureButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int clientId = (int)ClientComboBox.SelectedValue;  // ID du client sélectionné
                DateTime invoiceDate = DateFacturePicker.SelectedDate.Value;  // Date de la facture
                decimal total = decimal.Parse(TotalTextBox.Text);  // Total de la facture
                string status = ((ComboBoxItem)StatutComboBox.SelectedItem).Content.ToString();

                // Insérer la facture dans la base de données
                int invoiceId = Bdd.InsertInvoice(clientId, invoiceDate, total, status);

                // Ajouter les lignes de facture
                foreach (var item in ProduitsDataGrid.Items)
                {
                    dynamic row = item;
                    int productId = row.Produit.Id;  // Récupérer l'ID du produit
                    int quantity = row.Quantite;     // Quantité
                    decimal unitPrice = row.PrixUnitaire;  // Prix unitaire

                    // Insérer la ligne de facture dans la base de données
                    Bdd.InsertInvoiceLine(invoiceId, productId, quantity, unitPrice);
                }

                MessageBox.Show("Facture enregistrée avec succès.");
                this.Close();  // Fermer la fenêtre après enregistrement
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'enregistrement de la facture : {ex.Message}");
            }
        }

        // Annuler l'ajout de la facture
        private void AnnulerButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();  // Fermer la fenêtre sans enregistrer
        }

        private void ModifierButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FactureIDTextBox.Text))
            {
                MessageBox.Show("Veuillez sélectionner une facture à modifier.");
                return;
            }
            int invoiceId = int.Parse(FactureIDTextBox.Text);

            var invoice = Bdd.GetInvoiceById(invoiceId);
            if (invoice != null)
            {
                ClientComboBox.SelectedValue = invoice.ClientId;
                DateFacturePicker.SelectedDate = invoice.Date;
                TotalTextBox.Text = invoice.Total.ToString("F2");
                foreach (ComboBoxItem item in StatutComboBox.Items)
                {
                    if (item.Content.ToString() == invoice.Status)
                    {
                        StatutComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                MessageBox.Show("Facture introuvable.");
            }
        }
    }
}