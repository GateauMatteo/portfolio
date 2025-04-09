using System;

namespace Infotols
{
    public class RendezVous
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int SalespersonId { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; }
        public Status Status { get; set; }

        public override string ToString()
        {
            return $"{Date.ToShortDateString()} - {Location} - {Status}";
        }
    }

    // Enumération pour les statuts des rendez-vous
    public enum Status
    {
        Planned,
        Realized,
        Canceled
    }
}
