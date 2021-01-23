using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using SharpGL.SceneGraph;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                Console.WriteLine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Camera"), "camera.obj", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
            visina_zatvora.Text = m_world.cage_height.ToString() + " ";
            rotate.Text = m_world.rotation_speed.ToString() + " ";
            skaliranje.Text = m_world.scale_factor.ToString() + " ";
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
            visina_zatvora.Text = m_world.cage_height.ToString() + " ";
            rotate.Text = m_world.rotation_speed.ToString() + " ";
            skaliranje.Text = m_world.scale_factor.ToString() + " ";
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F6: this.Close(); break;
                case Key.Up:
                    if (!m_world.m_startAnimation) {
                        if (m_world.RotationX - 7.0f >= -60)
                            m_world.RotationX -= 7.0f;
                        else
                            MessageBox.Show("Nemoguce izvrsiti rotaciju, scena maksimalno rotirana", "GRESKA", MessageBoxButton.OK);
                    }
                    break;
                case Key.Down:
                    if (!m_world.m_startAnimation)
                    {
                        if (m_world.RotationX + 7.0f <= 150)
                            m_world.RotationX += 7.0f;
                        else
                            MessageBox.Show("Nemoguce izvrsiti rotaciju, scena maksimalno rotirana", "GRESKA", MessageBoxButton.OK);
                    }
                    break;
                case Key.Left:
                    if (!m_world.m_startAnimation)
                    {
                        if (m_world.RotationY - 7.0f >= -120)
                            m_world.RotationY -= 7.0f;
                        else
                            MessageBox.Show("Nemoguce izvrsiti rotaciju, scena maksimalno rotirana", "GRESKA", MessageBoxButton.OK);
                    }
                    break;
                case Key.Right:
                    if (!m_world.m_startAnimation)
                    {
                        if (m_world.RotationY + 7.0f <= 60)
                            m_world.RotationY += 7.0f;
                        else
                            MessageBox.Show("Nemoguce izvrsiti rotaciju, scena maksimalno rotirana", "GRESKA", MessageBoxButton.OK);
                    }
                    break;
                case Key.Add:
                    if (!m_world.m_startAnimation)
                    {
                        m_world.SceneDistance -= 250.0f;
                    }
                    break;
                case Key.Subtract:
                    if (!m_world.m_startAnimation)
                    {
                        m_world.SceneDistance += 250.0f;
                    }
                    break;
                case Key.M:
                    if (!m_world.m_startAnimation)
                    {
                        m_world.m_startAnimation = true;
                    }
                    break;
                case Key.F2:
                    if (!m_world.m_startAnimation)
                    {
                        OpenFileDialog opfModel = new OpenFileDialog();
                        bool result = (bool) opfModel.ShowDialog();
                        if (result)
                        {

                            try
                            {
                                World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                                m_world.Dispose();
                                m_world = newWorld;
                                m_world.Initialize(openGLControl.OpenGL);
                            }
                            catch (Exception exp)
                            {
                                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK);
                            }
                        }
                    }
                    break;
            }
        }

        private void buttonPlus_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.m_startAnimation)
            {
                m_world.cage_height += 10f;
                visina_zatvora.Text = m_world.cage_height.ToString() + " ";
            }
        }

        private void buttonMinus_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.m_startAnimation)
            {
                if (m_world.cage_height > 10f)
                {
                    m_world.cage_height -= 10f;
                    visina_zatvora.Text = m_world.cage_height.ToString() + " ";
                }
                else
                {
                    MessageBox.Show("Visina kaveza je: " + m_world.cage_height.ToString() + " Nije moguce dodatno je smanjiti", "GRESKA", MessageBoxButton.OK);
                }
            }
        }

        private void s_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.m_startAnimation)
            {
                if (m_world.rotation_speed >= 1f)
                {
                    m_world.rotation_speed -= 1f;
                    rotate.Text = m_world.rotation_speed.ToString() + " ";
                }
                else
                {
                    MessageBox.Show("Brzina rotacije je: " + m_world.rotation_speed.ToString() + " Nije moguce dodatno je smanjiti", "GRESKA", MessageBoxButton.OK);
                }
            }
            
        }

        private void a_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.m_startAnimation)
            {
                m_world.rotation_speed += 1f;
                rotate.Text = m_world.rotation_speed.ToString() + " ";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!m_world.m_startAnimation)
            {
                m_world.scale_factor += 0.1f;
                skaliranje.Text = m_world.scale_factor.ToString() + " ";
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            if (!m_world.m_startAnimation)
            {
                if (m_world.scale_factor > 0.1f)
                {
                    m_world.scale_factor -= 0.1f;
                    skaliranje.Text = m_world.scale_factor.ToString() + " ";
                }
                else
                {
                    MessageBox.Show("Faktor skaliranja je: " + m_world.scale_factor.ToString() + " Nije moguce dodatno ga smanjiti", "GRESKA", MessageBoxButton.OK);
                }
            }
        }
    }
}
