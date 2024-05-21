using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Control de usuario está documentada en https://go.microsoft.com/fwlink/?LinkId=234236

namespace trainingApp
{
    public sealed partial class ExerciseCard : UserControl
    {
        public event EventHandler Accepted;

        public ExerciseCard()
        {
            this.InitializeComponent();
        }

        public string Nombre
        {
            get => ExerciseName.Text;
            set => ExerciseName.Text = value;
        }

        public string Descripcion
        {
            get => Description.Text;
            set => Description.Text = value;
        }

        public int Repeticiones
        {
            get => int.TryParse(Repetitions.Text, out var reps) ? reps : 0;
            set => Repetitions.Text = value.ToString();
        }

        public int Series
        {
            get => int.TryParse(Sets.Text, out var sets) ? sets : 0;
            set => Sets.Text = value.ToString();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            Accepted?.Invoke(this, EventArgs.Empty);
        }
    }
}
