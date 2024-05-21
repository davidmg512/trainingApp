using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trainingApp.Modelo
{
    public class Rutina
    {
        public int Id { get; set; }
        public string DiaDeSemana { get; set; }
        public ICollection<Ejercicio> Ejercicios { get; set; } = new ObservableCollection<Ejercicio>();
    }
}
