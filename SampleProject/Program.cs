using System;
using System.Reflection;
using System.Windows.Forms;
namespace SampleProject
{

   
    static class Program
    {
        [STAThread]
        static void Main()
        {
            VisualizationDemo demo = new VisualizationDemo();
            demo.Run(60);
        }
    }
}
