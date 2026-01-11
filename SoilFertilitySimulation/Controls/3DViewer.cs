using System;
using System.Diagnostics;
using System.IO;
using OpenTK.Graphics.ES30;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace SoilFertilitySimulation.Controls
{
    public class _3DViewer : GameWindow
    {
        private readonly ViewModels.SimulationViewModel ReferenceOfSimulationVM;

        public _3DViewer(int width, int height, string title, ViewModels.SimulationViewModel VM): base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title=title})
        {
            ReferenceOfSimulationVM = VM;
        }

        //Pipeline
        float[] vertices =
        {
            0.5f, 0.5f, 0.0f, 1.0f, 0.0f, 0.0f,
            0.5f, -0.5f, 0.0f, 0.0f, 1.0f, 0.0f,
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, 1.0f,
            -0.5f, 0.7f, 0.0f, 0.0f, 0.0f, 0.0f
        };
        uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private int VertexBufferObject;
        private int ElementBufferObject;
        private int VertexArrayObject;
        private int ShaderHandle;

        //OpenGl Commands
        protected override void OnLoad()
        {
            base.OnLoad();

            ReferenceOfSimulationVM.Is3DViewInactive = false;

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            //Vertex Specification
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            VertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            ElementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            //Shaders
            //Add catch just in case the file doesn't exist
            string VertexShaderSource = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, @"Shaders\shader.vert"));
            string FragmentShaderSource = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, @"Shaders\shader.frag"));
            int VertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(VertexShader, VertexShaderSource);
            GL.CompileShader(VertexShader);

            GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success1);
            if (success1 == 0)
            {
                Debug.Write(GL.GetShaderInfoLog(VertexShader));
            }

            int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentShader, FragmentShaderSource);
            GL.CompileShader(FragmentShader);

            GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int success2);
            if (success2 == 0)
            {
                Debug.Write(GL.GetShaderInfoLog(FragmentShader));
            }

            ShaderHandle = GL.CreateProgram();
            GL.AttachShader(ShaderHandle, VertexShader);
            GL.AttachShader(ShaderHandle, FragmentShader);
            GL.LinkProgram(ShaderHandle);
            GL.DetachShader(ShaderHandle, VertexShader);
            GL.DeleteShader(VertexShader);
            GL.DetachShader(ShaderHandle, FragmentShader);
            GL.DeleteShader(FragmentShader);

        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (KeyboardState.IsKeyDown(Keys.Escape))
            {
                Close();
            }

        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(ShaderHandle);

            GL.BindVertexArray(VertexArrayObject);
            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();
        }


        protected override void OnUnload()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);
            GL.DeleteBuffer(VertexBufferObject);
            GL.DeleteVertexArray(VertexArrayObject);
            GL.DeleteProgram(ShaderHandle);

            ReferenceOfSimulationVM.Is3DViewInactive = true;

            base.OnUnload();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }
    }
}
