using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trainingApp.Modelo;

namespace trainingApp.ViewModel
{
    public class MainViewModel
    {
        public ObservableCollection<Ejercicio> EjerciciosLunes { get; set; }

        public MainViewModel()
        {
            // Inicializa la colección de ejercicios para el Lunes
            EjerciciosLunes = new ObservableCollection<Ejercicio>();
        }
    }
}
