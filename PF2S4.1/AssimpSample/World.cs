// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using SharpGL.Enumerations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 2500.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private enum TextureObjects { BRICKS = 0, RUST, CONCRETE };

        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        private uint[] m_textures = null;

        public string[] m_textureFiles = { "..//..//Textures//bricks.jpg", "..//..//Textures//rust.jpg", "..//..//Textures//concrete.jpg" };

        public int stop = 1;
        public int smer_animacije = 1;
        public float cage_height = 300f;
        public float wall_height = 280f;
        public float rotation_speed = 1.0f;
        public float scale_factor = 1f;
        public Boolean m_startAnimation = false;
        public Boolean redLight = true;
        public float cameraRotation = 90.0f;
        public float doorRotation = 0.0f;

        private DispatcherTimer animationTimer;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
            m_textures = new uint[m_textureCount];
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            LoadTexture(gl);
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE_MODE);
            SetupLighting(gl);
            // Ukljucivanje color-tracking mehanizma
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);
            m_scene.LoadScene();
            m_scene.Initialize();
            animationTimer = new System.Windows.Threading.DispatcherTimer();
            animationTimer.Interval = TimeSpan.FromMilliseconds(5);
            animationTimer.Tick += new EventHandler(UpdateAnimation);
        }

        private void SetupLighting(OpenGL gl)
        {
            // Definisanje tackastog izvora svetla
            gl.Enable(OpenGL.GL_LIGHTING);

            float[] amb_comp = {0.4f, 0.4f, 0.4f, 1.0f };
            float[] diff_comp = { 0.6f, 0.6f, 0.6f, 1.0f };
            float[] amb_light = { 1.0f, 1.0f, 1.0f, 1.0f };
            float[] positon = { -700.0f, 0.0f, -2500.0f, 1.0f };

            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT, amb_light);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 128.0f);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, amb_comp);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, diff_comp);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, positon);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_NORMALIZE);
            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        public void SetupReflectorLighting(OpenGL gl, Boolean enabled) {
            if (enabled) {
                float[] ambijentalnaKomponenta = { 0.2f, 0.0f, 0.0f, 1.0f };
                float[] difuznaKomponenta = { 0.8f, 0.0f, 0.0f, 1.0f };
                float[] smer = { 0.0f, 0.0f, 1.0f };
                // Pridruži komponente svetlosnom izvoru 1
                gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT,
                 ambijentalnaKomponenta);
                gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE,
                difuznaKomponenta);
                // Podesi parametre reflektorkskog izvora
                gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);
                gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 25.0f);
                // Ukljuci svetlosni izvor
                gl.Enable(OpenGL.GL_LIGHT1);
                // Pozicioniraj svetlosni izvor
                float[] pozicija = { 0.0f, 0.0f, 0.0f, 1.0f };
                gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, pozicija);
            }
            else
            {
                gl.Disable(OpenGL.GL_LIGHT1);
            }
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            if (m_startAnimation)
            {
                animationTimer.Start();
            }
            else
            {
                animationTimer.Stop();
                cameraRotation = 0.0f;
                doorRotation = 0.0f;
            }

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            // Pozicioniranje kamere
            gl.LookAt(-500, 300, 1000, 0, 0, 0, 0, 1, 0);

            gl.PushMatrix();
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD);
            gl.Translate(0f, 250f, -380f);
            gl.Scale(scale_factor * 20f, scale_factor * 20f, scale_factor * 20f);
            gl.Rotate(30f, cameraRotation, 0f);
            SetupReflectorLighting(gl, redLight);
            m_scene.Draw();
            gl.PopMatrix();

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.CONCRETE]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            DrawBase(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.BRICKS]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            DrawWalls(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            
            DrawCage(gl);

            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.RUST]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            DrawDoor(gl);
            gl.Disable(OpenGL.GL_TEXTURE_2D);

            gl.PopMatrix();
            DrawText(gl);
            gl.Flush();
        }

        public void DrawDoor(OpenGL gl) {
            gl.PushMatrix();
            gl.Translate(0f, cage_height/2 - 245f, -285f);
            if (doorRotation > 0)
            {
                gl.Translate(45f, 0f, -45f);
            }
            gl.Rotate(0.0f, doorRotation, 0.0f);
            if(doorRotation > 0)
            {
                gl.Translate(-45f, 0f, 0f);
            }
            gl.Scale(90f, cage_height/2, 4f);
            Cube cube = new Cube();
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        public void DrawBase(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Color(1f, 1f, 1f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.TexCoord(0, 0);
            gl.Vertex(560.0f, -250.0f, -460.0f);
            gl.TexCoord(1, 0);
            gl.Vertex(-560.0f, -250.0f, -460.0f);
            gl.TexCoord(1, 1);
            gl.Vertex(-560.0f, -250.0f, 460.0f);
            gl.TexCoord(0, 1);
            gl.Vertex(560.0f, -250.0f, 460.0f);
            gl.End();
            gl.PopMatrix();
        }

        public void DrawWalls(OpenGL gl)
        {
            //Zadnji zid
            gl.PushMatrix();
            gl.Color(0.625f, 0.25f, 0.125f);
            gl.Translate(0f, wall_height - 250f, -480f);
            gl.Scale(600f, wall_height, 20f);
            Cube cube = new Cube();
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            //Prednji zid
            /*
            gl.PushMatrix();
            gl.Color(0.625f, 0.25f, 0.125f);
            gl.Translate(0f, wall_height - 250f, 480f);
            gl.Scale(600f, wall_height, 20f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            */
            //Desni zid
            gl.PushMatrix();
            gl.Color(0.6f, 0.2f, 0.1f);
            gl.Translate(580f, wall_height - 250f, 0f);
            gl.Scale(20f, wall_height, 460f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
            //Levi zid
            gl.PushMatrix();
            gl.Color(0.6f, 0.2f, 0.1f);
            gl.Translate(-580f, wall_height - 250f, 0f);
            gl.Scale(20f, wall_height, 460f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        public void DrawCage(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.RUST]);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.Scale(.1f, .1f, .1f);
            gl.LoadIdentity();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            Cylinder cage = new Cylinder();
            cage.TextureCoords = true;
            gl.Translate(0f, cage_height - 245f, 0f);
            gl.Color(0.3f, 0.3f, 0.3f);
            cage.CreateInContext(gl);
            cage.NormalGeneration = Normals.Smooth;
            cage.NormalOrientation = Orientation.Outside;
            gl.Scale(300f, cage_height, 300f);
            cage.TopRadius = 1;
            gl.Rotate(90.0f, 0.0f, 0.0f);
            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
            gl.LineWidth(8.0f);
            cage.Render(gl, RenderMode.Render);
            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.PopMatrix();
        }

        public void DrawText(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Disable(OpenGL.GL_DEPTH_TEST);
            string[] text = { "Predmet: Racunarska grafika", "Sk.god: 2020/21.", "Ime: Aleksandar", "Prezime: Stevanovic", "Sifra zad: 4.1" };
            string[] underlines = { "______________________", "_____________", "_____________", "________________", "__________" };
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Viewport(0, m_height / 2, m_width / 2, m_height / 2);
            gl.Ortho2D(-20, 20, -20, 20);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.Color(1.0f, 0.0f, 1.0f);
            gl.Translate(-18f, 18f, 0.0f);
            gl.Scale(2f, 2f, 2f);
            float step_text = 0f;
            float step_underline = -0.2f;
            for (int i = 0; i < text.Length; i++)
            {
                step_text -= 1.2f;
                step_underline -= 1.2f;
                gl.PushMatrix();
                gl.Translate(0f, step_text, 0f);
                gl.DrawText3D("Arial", 12f, 0.0f, 0.1f, text[i]);
                gl.PopMatrix();
                gl.PushMatrix();
                gl.Translate(0f, step_underline, 0f);
                gl.Scale(1.05f, 1f, 1f);
                gl.DrawText3D("Arial", 12f, 0.0f, 0.1f, underlines[i]);
                gl.PopMatrix();

            }
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.PopMatrix();
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Perspective(50f, (double)m_width / m_height, 0.5f, 5000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

        }

        private void UpdateAnimation(object sender, EventArgs e)
        {
            if (stop <= 360) {
                if (cameraRotation > -60.0f && cameraRotation <= 60.0f)
                {
                    cameraRotation += smer_animacije * rotation_speed;
                    stop += 1;
                }
                else
                {
                    cameraRotation -= smer_animacije * rotation_speed;
                    smer_animacije *= -1;
                    stop += 1;
                }
            }
            if (stop > 360 && stop <= 540)
            {
                if (stop % 10 == 0) {
                    redLight = !redLight;
                }
                stop += 1;
                doorRotation = -45.0f;
                cameraRotation = 0.0f;
            }
            if (stop > 540) {
                redLight = true;
                stop = 0;
                m_startAnimation = false;
            }
        }

        private void LoadTexture(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.GenTextures(m_textureCount, m_textures);

            for (int i = 0; i < m_textureCount; ++i)
            {
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);
                Bitmap image = new Bitmap(m_textureFiles[i]);
                image.RotateFlip(RotateFlipType.Rotate90FlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                BitmapData imageData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);
                image.UnlockBits(imageData); image.Dispose();
            }
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, width, height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(50f, (double)width / height, 0.5f, 5000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
