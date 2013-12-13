﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Windows.Forms;

using NormalModeLibrary.Sources;
using NormalModeLibrary.ViewModel;

namespace NormalModeLibrary
{
    public class ComponentFactory
    {
        static ComponentFactory factory;
        static readonly string FilePath = AppDomain.CurrentDomain.BaseDirectory + @"Project\CurrentModePanel.xml";
        Form mainMnemoHandle;
        readonly List<UserViewModel> users = new List<UserViewModel>();
        readonly List<Windows.ViewWindow> activePanelForm = new List<Windows.ViewWindow>();
        bool isLoad = false;

        private ComponentFactory() { }
        private List<PanelViewModel> GetPanels( Places place )
        {
            var panels = new List<PanelViewModel>();
            foreach ( UserViewModel user in users )
                if ( user.Login == HMI_MT_Settings.HMI_Settings.UserName )
                    foreach ( ConfigurationViewModel config in user.Collection )
                        if ( config.ActiveTime == user.ActiveTime && config.IsActive && config.Place == place )
                            foreach ( PanelViewModel panel in config.Collection )
                                if ( panel.Collection.Count > 0 )
                                    panels.Add( panel );
            return panels;
        }        
        public void LoadXml()
        {
            if ( !isLoad )
            {
                var xdoc = XDocument.Load( FilePath );
                var nodes = xdoc.Element( "MTRA" ).Elements( "User" );

                foreach ( var node in nodes )
                {
                    var user = new User();
                    user.ParseXml( node );
                    users.Add( new UserViewModel( user ) );
                }

                isLoad = true;
            }
        }
        public bool SaveXml()
        {
            return SaveXml( users );
        }
        public void ActivatedMainMnemoForms( Form form )
        {
            if ( activePanelForm.Count > 0 )
                DeactivatedMainMnemoForms();
           
            mainMnemoHandle = form;

            foreach ( var vmPanel in GetPanels( Places.MainMnemo ) )
            {
                var view = new Windows.ViewWindow { Component = vmPanel, Place = Places.MainMnemo };
                activePanelForm.Add( view );
                view.Owner = mainMnemoHandle;
                //view.ActivatedComponent();
                view.Show();
            }
        }
        public void DeactivatedMainMnemoForms()
        {
            if ( activePanelForm.Count > 0 )
                foreach ( Windows.ViewWindow view in activePanelForm.ToArray() )
                    if ( view.Place == Places.MainMnemo )
                    {
                        view.DeactivatedComponent();
                        activePanelForm.Remove( view );
                    }
        }
        public void SetStates( FormWindowState state )
        {
            foreach ( Windows.ViewWindow view in activePanelForm )
                switch ( state )
                {
                    case FormWindowState.Maximized: view.WindowState = FormWindowState.Normal; break;
                    default: view.WindowState = state; break;
                }
        }

        public static void EditSignals( InterfaceLibrary.IDevice device, String login, Places place )
        {
            var win = new Windows.SelectSignalsWindow();
            var user = GetUser( Factory.users, login );
            var config = GetConfig( user, place );
            var panel = GetPanel( config, device.UniObjectGUID );

            win.AddComponents( device, panel );

            if ( win.ShowDialog() == DialogResult.OK )
            {
                var view = Factory.activePanelForm.FirstOrDefault( apf => apf.Component == panel );
                if ( view == null && panel.Collection.Count > 0 )
                {
                    view = new Windows.ViewWindow { Component = panel, Place = place };
                    Factory.activePanelForm.Add( view );
                    view.Owner = Factory.mainMnemoHandle;
                }

                if ( view != null )
                {
                    if ( panel.Collection.Count == 0 )
                    {
                        view.Close();
                        Factory.activePanelForm.Remove( view );
                        return;
                    }

                    //view.ActivatedComponent();
                    try
                    {
                        //view.ActivatedComponent();
                        view.Show();
                    }
                    catch ( Exception )
                    {
                        view.DeactivatedComponent();
                        Factory.activePanelForm.Remove( view );
                        var newView = new Windows.ViewWindow { Component = panel, Place = place };
                        Factory.activePanelForm.Add( newView );
                        newView.Owner = Factory.mainMnemoHandle;
                        newView.Show();
                    }
                }
            }

        }
        public static void EditUserWindows()
        {
            var win = new Windows.UserWindows( Factory.users ) { Owner = Factory.mainMnemoHandle };
            win.Show();
        }

        internal static bool SaveXml( List<UserViewModel> users )
        {
            var xdoc = new XDocument( new XDeclaration( "1.0", "utf-8", "true" ) );
            var node = new XElement( "MTRA" );
            xdoc.Add( node );

            foreach ( UserViewModel userModel in users )
                node.Add( userModel.Core.CreateXml() );

            try { xdoc.Save( FilePath ); }
            catch { return false; }

            return true;
        }
        private static UserViewModel GetUser( List<UserViewModel> users, String login )
        {
            var userModel = users.FirstOrDefault( u => u.Login == login );
            if ( userModel == null )
            {
                var user = new User { Login = login };
                userModel = new UserViewModel( user );
                users.Add( userModel );
            }
            return userModel;
        }
        private static ConfigurationViewModel GetConfig( UserViewModel model, Places place )
        {
            ConfigurationViewModel configModel = null;
            foreach ( ConfigurationViewModel cfgModel in model.Collection )
                if ( cfgModel.IsActive && cfgModel.ActiveTime == model.ActiveTime && cfgModel.Place == place )
                {
                    configModel = cfgModel;
                    break;
                }

            if ( configModel == null )
            {
                var config = new Configuration { ActiveTime = model.ActiveTime, Place = place };

                ( (User)model.Core ).Collection.Add( config );
                configModel = new ConfigurationViewModel( config );
                model.Collection.Add( configModel );
            }

            return configModel;
        }
        private static PanelViewModel GetPanel( ConfigurationViewModel model, UInt32 guid )
        {
            PanelViewModel panel = null;
            foreach ( PanelViewModel panelModel in model.Collection )
                if ( panelModel.ObjectGuid == guid )
                {
                    panel = panelModel;
                    break;
                }

            if ( panel == null )
            {
                var pnl = new Sources.Panel { ObjectGuid = guid, Type = Sources.Panel.LinkType.Named };

                ( (Configuration)model.Core ).Collection.Add( pnl );
                panel = new PanelViewModel( pnl );
                model.Collection.Add( panel );
            }

            return panel;
        }

        public static ComponentFactory Factory
        {
            get { return factory ?? ( factory = new ComponentFactory( ) ); }
        }
    }
}
