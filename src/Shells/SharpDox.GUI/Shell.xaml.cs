﻿using SharpDox.GUI.Controls.ConfigGrid;
using SharpDox.GUI.ViewModels;
using SharpDox.Sdk.Build;
using SharpDox.Sdk.Config;
using SharpDox.Sdk.Exporter;
using SharpDox.Sdk.Local;
using SharpDox.Sdk.UI;
using System;
using System.Windows;
using System.Windows.Input;

namespace SharpDox.GUI
{
    public partial class Shell : IShell
    {
        public event Action OnClose;

        public Shell(IConfigController configController, IExporter[] allExporters, ILocalController localController, IBuildController buildController)
        {
            var guiStrings = localController.GetLocalStrings<SDGuiStrings>();
            DataContext = new ShellViewModel(guiStrings, configController, buildController, ExecuteOnClose);
            Strings = guiStrings;

            InitializeComponent();

            svBody.Content = new ConfigGridControl(configController, allExporters, localController, buildController);

            MouseLeftButtonDown += OnMouseDown;
            MouseLeftButtonUp += OnMouseUp;
        }

        public void Start(string[] args)
        {
            new Application();
            ShowDialog();
        }

        private void ExecuteOnClose()
        {
            if (OnClose != null) OnClose();
            Close();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // this prevents win7 aerosnap
                if (ResizeMode != System.Windows.ResizeMode.NoResize)
                {
                    ResizeMode = System.Windows.ResizeMode.NoResize;
                    UpdateLayout();
                }

                DragMove();
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (ResizeMode == System.Windows.ResizeMode.NoResize)
            {
                // restore resize grips
                ResizeMode = System.Windows.ResizeMode.CanResizeWithGrip;
                UpdateLayout();
            }
        }

        public SDGuiStrings Strings { get; private set; }
        public bool IsGui { get { return true; } }
    }
}