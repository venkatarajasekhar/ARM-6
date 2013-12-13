﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InterfaceLibrary;
using System.Collections;
using HMI_MT_Settings;
using System.Globalization;

using TypeBlockData4Req = InterfaceLibrary.TypeBlockData4Req;
using DeviceFormLib.BlockTabs;

namespace DeviceFormLib
{
    public partial class FormSirius : Form, IDeviceForm
    {
        TabPage tpCurrent;
        //readonly frmEngine frmengine;
        readonly FrmEngineNew engineNew;
        readonly UInt32 uniDev = 0xffffffff;
        /// <summary>
        /// признак того что вкладка уставок уже была сформирована
        /// </summary>
        Boolean isTpConfigEntered;
        /// <summary>
        /// признак того что вкладка срабатывания уже была сформирована
        /// </summary>
        Boolean isTpRunningEntered;
        /// <summary>
        /// флаг - покаывать ли сообщение об отсутсвии архивных записей 
        /// или нет
        /// </summary>
        Boolean isMesView;
        Boolean isCurrentInfoEnter;
        Boolean isOperationControlEnter;

        public FormSirius( UInt32 unids, UInt32 unidev )
        {
            InitializeComponent();

            try
            {
                uniDev = unidev;
                //frmengine = new frmEngine( unids, unidev, this );
                engineNew = new FrmEngineNew( unids, unidev, this );

                tabControl.Controls.Add( new EventBlockTabPage( unidev ) );
                tabControl.Controls.Add( new OscDiagTabPage( unidev, OscDiagTabPage.OscDiagPanelVisible.Oscilograms ) );
                tabControl.Controls.Add( new InformationTabPage( unidev ) );
                tabControl.Controls.Add( new DataBaseFilesLibrary.TabPageDbFile( uniDev ) );
            }
            catch ( Exception ex )
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( ex );
            }
        }
        private void FormSiriusDDeviceLoad( object sender, EventArgs e )
        {
            try
            {
                ////frmengine.InitFrm( this, tabControl, null );
                //frmengine.InitFrmLight( this, tabControl, null );
                InitInterfaceElementsClick();
            }
            catch ( Exception ex )
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( ex );
            }
        }
        private void FormSiriusDDeviceFormClosing( object sender, FormClosingEventArgs e )
        {
            // отписываемся от тегов
            HMI_Settings.HMIControlsTagReNew( tpCurrent, HMI_Settings.SubscribeAction.UnSubscribe );
        }

        public void CreateDeviceForm() { }
        public void InitInterfaceElementsClick()
        {
            tabControl.Selected += this.TabControlSelected;
            tpCurrentInfo.Enter += this.TpCurrentInfoEnter;
            tpRunning.Enter += this.TpRunningEnter;
            tpConfig.Enter += this.TpConfigEnter;
            tpOperationControl.Enter += this.TpOperationControlOnEnter;

            this.selectControl1.btnUpdate.Click += this.SelectControl1BtnUpdateClick;
            this.selectControl2.btnUpdate.Click += this.SelectControl2BtnUpdateClick;
            this.readWriteUstavky1.btnReadUstFC.Click += this.BtnReadUstFcClick;
            this.readWriteUstavky1.btnWriteUst.Click += this.BtnWriteUstClick;
            this.readWriteUstavky1.btnResetValues.Click += this.BtnResetValuesClick;
            this.readWriteUstavky1.btnFix4Change.CheckedChanged += BtnFix4ChangeOnCheckedChanged;

            this.lstvRun.ItemActivate += this.LstvAvarItemActivate;
            this.lstvConfig.ItemActivate += this.LstvConfigItemActivate;
        }
        /// <summary>
        /// активировать определенную вкладку
        /// </summary>
        /// <param name="typetabpage"></param>
        public void ActivateTabPage(string typetabpage)
        {
            try
            {
                switch (typetabpage)
                {
                    case "Авария":
                        tabControl.SelectedTab = tpRunning;
                        //tabControl1.SelectedTab = tbpAvar;
                        break;
                }
            }
            catch (Exception ex)
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG(ex);
            }
        }
        public void reqAvar_OnReqExecuted( IRequestData req ) { }
        /// <summary>
        /// Выбор вкладки
        /// </summary>
        private void TabControlSelected( object sender, TabControlEventArgs e )
        {
            HMI_Settings.HMIControlsTagReNew( tpCurrent, HMI_Settings.SubscribeAction.UnSubscribe );
            tpCurrent.Tag = false; // признак отписки от тегов для данной TabPage
            tpCurrent = e.TabPage;  // запомним новую текущую вкладку
        }
        private void TpCurrentInfoEnter( object sender, EventArgs e )
        {
            //CommonUtils.CommonUtils.RemoveHMIUserControls(tpCurrentInfo);
            if ( !isCurrentInfoEnter )
            {
                //frmengine.PlaceTagsOnTPFlps( "Контроль" );
                engineNew.PlaceTagsOnPanels( "Контроль" );
                isCurrentInfoEnter = true;
            }

            tpCurrent = HMI_Settings.SetTagsSubscribe4TPCurrent( tpCurrentInfo );
        }
        private void TpRunningEnter( object sender, EventArgs e )
        {
            if ( !isTpRunningEntered )
            {
                //frmengine.PlaceTagsOnTPFlps( "Срабатывание" );
                //frmengine.CreateTPData( tpRunning, pnlUstavCopy, "Копия уставок", false );
                engineNew.PlaceTagsOnPanels( "Срабатывание" );
                engineNew.CreateAutoTabPage( tabPage2, "Копия уставок", false );
                isTpRunningEntered = true;
            }

            tpCurrent = HMI_Settings.SetTagsSubscribe4TPCurrent( tpRunning );
        }
        private void TpConfigEnter( object sender, EventArgs e )
        {
            if ( !isTpConfigEntered )
            {
                //frmengine.CreateTPData( tpConfig, pnlTPConfig, "Уставки", true );
                engineNew.CreateAutoTabPage( pnlTPConfig, "Уставки", true );
                isTpConfigEntered = true;
            }

            tpCurrent = HMI_Settings.SetTagsSubscribe4TPCurrent( tpConfig );
        }
        private void TpOperationControlOnEnter( object sender, EventArgs eventArgs )
        {
            if ( !isOperationControlEnter )
            {
                //frmengine.PlaceTagsOnTPFlps( "Оперативное управление(дубль)" );
                engineNew.PlaceTagsOnPanels( "Оперативное управление(дубль)" );
                isOperationControlEnter = true;
            }

            tpCurrent = HMI_Settings.SetTagsSubscribe4TPCurrent( tpOperationControl );
        }

        /// <summary>
        /// вывод информации при выборе конкретной аварии (Информация об авариях)
        /// </summary>
        private void LstvAvarItemActivate( object sender, EventArgs e )
        {
            try
            {
                if ( lstvRun.SelectedItems.Count == 0 )
                    return;

                var idBlock = (int)lstvRun.SelectedItems[0].Tag;
                var arparam = new ArrayList
                {
                    idBlock, Encoding.UTF8.GetBytes( HMI_Settings.ProviderPtkSql )
                };

                /*var req = */HMI_Settings.CONFIGURATION.GetData( 0, uniDev, "ArhivAvariBlockData", arparam, idBlock );
                //req.OnReqExecuted += reqAvar_OnReqExecuted;
            }
            catch ( Exception ex )
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( ex );
            }
        }
        /// <summary>
        /// выбор конкретной записи из списка
        /// </summary>
        private void LstvConfigItemActivate( object sender, EventArgs e )
        {
            try
            {
                if ( lstvConfig.SelectedItems.Count == 0 )
                    return;

                var idBlock = (int)lstvConfig.SelectedItems[0].Tag;
                var arparam = new ArrayList
                {
                    idBlock, Encoding.UTF8.GetBytes( HMI_Settings.ProviderPtkSql )
                };

                /*IRequestData req = */HMI_Settings.CONFIGURATION.GetData( 0, uniDev, "ArhivUstavkiBlockData", arparam, idBlock );
                HMI_Settings.SetTagsSubscribe4TPCurrent( tpConfig );
                //req.OnReqExecuted += new ReqExecuted( reqAvar_OnReqExecuted );
            }
            catch ( Exception ex )
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( ex );
            }
        }
        /// <summary>
        /// Срабатывание
        /// </summary>
        private void SelectControl1BtnUpdateClick( object sender, EventArgs e )
        {
            isMesView = true;

            var list = HelperControlsLibrary.FrmEngine.AvarUstavkyDataBase( uniDev, isMesView,
                                                                            selectControl1.StartDateCollapsed,
                                                                            selectControl1.EndTimeCollapsed,
                                                                            TypeBlockData4Req.TypeBlockData4Req_Srabat );
            if ( list.Count == 0 )
            {
                isMesView = false;
                MessageBox.Show( "Архивных данных для этого устройства нет.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
            else
            {
                lstvRun.Items.Clear();
                lstvRun.Items.AddRange( list.ToArray() );
            }
        }
        /// <summary>
        /// Конфигурация и уставки
        /// </summary>
        private void SelectControl2BtnUpdateClick( object sender, EventArgs e )
        {
            isMesView = true;

            var list = HelperControlsLibrary.FrmEngine.AvarUstavkyDataBase( uniDev, isMesView,
                                                                            selectControl2.StartDateCollapsed,
                                                                            selectControl2.EndTimeCollapsed,
                                                                            TypeBlockData4Req.TypeBlockData4Req_Ustavki );
            if ( list.Count == 0 )
            {
                isMesView = false;
                MessageBox.Show( "Архивных данных для этого устройства нет.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
            else
            {
                lstvConfig.Items.Clear();
                lstvConfig.Items.AddRange( list.ToArray() );
            }
        }
        /// <summary>
        /// Чтение уставок
        /// </summary>
        private void BtnReadUstFcClick( object sender, EventArgs e )
        {
            // правильная запись в журнал действий пользователя номер устройства с цчетом фк
            CommonUtils.CommonUtils.WriteEventToLog( 7, uniDev.ToString( CultureInfo.InvariantCulture ), true );
            HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "IMP", new byte[] { }, this );

            //HMI_Settings.HMIControlsTagReNew( tpConfig, HMI_Settings.SubscribeAction.Subscribe );
            HMI_Settings.SetTagsSubscribe4TPCurrent( tpConfig );
        }
        /// <summary>
        /// Запись уставок
        /// </summary>
        private void BtnWriteUstClick( object sender, EventArgs e )
        {
            try
            {
                // определим теги которые изменились
                var lstModifiedTags = engineNew.GetChangedTags( pnlTPConfig );//frmengine.GetList4WriteUst();

                if ( !lstModifiedTags.Any() )
                {
                    MessageBox.Show( "Уставки не изменялись. \nВыполнение команды отменено.", "Сообщение", MessageBoxButtons.OK, MessageBoxIcon.Information );
                    HMI_Settings.SetTagsSubscribe4TPCurrent( tpConfig );
                    return;
                }

                if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b05_Set_ustav_config, HMI_Settings.UserRight ) )
                {
                    HMI_Settings.SetTagsSubscribe4TPCurrent( tpConfig );
                    return;
                }

                if ( HMI_Settings.isRegPass && CommonUtils.CommonUtils.CanAction() )
                {
                    if ( MessageBox.Show( "Записать уставки?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.No )
                    {
                        HMI_Settings.SetTagsSubscribe4TPCurrent( tpConfig );
                        return;
                    }
                    // правильная запись в журнал действий пользователя номер устройства с учетом фк
                    CommonUtils.CommonUtils.WriteEventToLog( 6, uniDev.ToString( CultureInfo.InvariantCulture ), true );
                    //"выдана команда WCP - запись уставок."

                    HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "WCP",
                                                               CommonUtils.CommonUtils.GetUstConfMemXPacket(
                                                                   (List<ITag>)lstModifiedTags ), this );

                    // сбрасываем признаки изменения тега
                    CommonUtils.CommonUtils.ResetIndicationModifiedTag( tpConfig );
                    HMI_Settings.SetTagsSubscribe4TPCurrent( tpConfig );
                }
            }
            catch ( Exception ex )
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( ex );
            }
        }
        /// <summary>
        /// Очистить поля формы
        /// </summary>
        private void BtnResetValuesClick( object sender, EventArgs e )
        {
            // отписались и обнулились
            HMI_Settings.HMIControlsTagReNew( tpConfig, HMI_Settings.SubscribeAction.UnSubscribeAndClear );
            tpConfig.Tag = false;
        }
        /// <summary>
        /// Проверка режима задания уставок
        /// </summary>
        private void BtnFix4ChangeOnCheckedChanged( object sender, EventArgs eventArgs )
        {
            var chBox = (CheckBox)sender;
            if ( chBox.Checked )
            {
                HMI_Settings.HMIControlsTagReNew( this.tpConfig, HMI_Settings.SubscribeAction.UnSubscribe );
                this.tpConfig.Tag = false;
            }
            else
            {
                var lstModifiedTags = engineNew.GetChangedTags( pnlTPConfig );//this.frmengine.GetList4WriteUst();
                if ( !lstModifiedTags.Any() )
                    HMI_Settings.SetTagsSubscribe4TPCurrent( tpConfig );
            }
        }
        /// <summary>
        /// Выключатель вкл.
        /// </summary>
        private void Button1Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b00_Control_Switch, HMI_Settings.UserRight ) )
                return;

            if ( HMI_Settings.isRegPass && !CommonUtils.CommonUtils.CanAction() )
                return;

            if ( MessageBox.Show( "Включить?", "Включение выключателя", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "CCB", new byte[] { }, this );
        }
        /// <summary>
        /// Выключатель откл.
        /// </summary>
        private void Button4Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b00_Control_Switch, HMI_Settings.UserRight ) )
                return;

            if ( HMI_Settings.isRegPass && !CommonUtils.CommonUtils.CanAction() )
                return;

            if ( MessageBox.Show( "Отключить?", "Отключение выключателя", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "OCB", new byte[] { }, this );
        }
        /// <summary>
        /// Квитировать
        /// </summary>
        private void Button6Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b02_ACK_Signaling, HMI_Settings.UserRight ) )
                return;
            
            if ( MessageBox.Show( "Квитировать?", "Квитирование", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "ECC", new byte[] { }, this );
        }
        /// <summary>
        /// ЗМН ввести
        /// </summary>
        private void Button7Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b00_Control_Switch, HMI_Settings.UserRight ) )
                return;

            if ( HMI_Settings.isRegPass && !CommonUtils.CommonUtils.CanAction() )
                return;

            if ( MessageBox.Show( "Выполнить ввод?", "Попытка ввода ЗМН", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */
            HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "ZN1", new byte[] { }, this );
        }
        /// <summary>
        /// ЗМН вывести
        /// </summary>
        private void Button5Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b00_Control_Switch, HMI_Settings.UserRight ) )
                return;

            if ( HMI_Settings.isRegPass && !CommonUtils.CommonUtils.CanAction() )
                return;

            if ( MessageBox.Show( "Выполнить вывод?", "Попытка вывода ЗМН", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */
            HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "ZN0", new byte[] { }, this );
        }
        /// <summary>
        /// АПВ ввести
        /// </summary>
        private void Button9Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b00_Control_Switch, HMI_Settings.UserRight ) )
                return;

            if ( HMI_Settings.isRegPass && !CommonUtils.CommonUtils.CanAction() )
                return;

            if ( MessageBox.Show( "Выполнить ввод?", "Попытка ввода АПВ", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */
            HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "AV1", new byte[] { }, this );
        }
        /// <summary>
        /// АПВ вывести
        /// </summary>
        private void Button8Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b00_Control_Switch, HMI_Settings.UserRight ) )
                return;

            if ( HMI_Settings.isRegPass && !CommonUtils.CommonUtils.CanAction() )
                return;

            if ( MessageBox.Show( "Выполнить вывод?", "Попытка вывода АПВ", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */
            HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "AV0", new byte[] { }, this );
        }
        /// <summary>
        /// УРОВ ввести
        /// </summary>
        private void Button11Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b00_Control_Switch, HMI_Settings.UserRight ) )
                return;

            if ( HMI_Settings.isRegPass && !CommonUtils.CommonUtils.CanAction() )
                return;

            if ( MessageBox.Show( "Выполнить ввод?", "Попытка ввода УРОВ", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */
            HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "UV1", new byte[] { }, this );
        }
        /// <summary>
        /// УРОВ вывести
        /// </summary>
        private void Button10Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b00_Control_Switch, HMI_Settings.UserRight ) )
                return;

            if ( HMI_Settings.isRegPass && !CommonUtils.CommonUtils.CanAction() )
                return;

            if ( MessageBox.Show( "Выполнить вывод?", "Попытка вывода УРОВ", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */
            HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "UV0", new byte[] { }, this );
        }
        /// <summary>
        /// АЧР ввести
        /// </summary>
        private void Button13Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b00_Control_Switch, HMI_Settings.UserRight ) )
                return;

            if ( HMI_Settings.isRegPass && !CommonUtils.CommonUtils.CanAction() )
                return;

            if ( MessageBox.Show( "Выполнить ввод?", "Попытка ввода АЧР", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */
            HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "AR1", new byte[] { }, this );
        }
        /// <summary>
        /// АЧР вывести
        /// </summary>
        private void Button12Click( object sender, EventArgs e )
        {
            if ( CommonUtils.CommonUtils.IsUserActionBan( CommonUtils.CommonUtils.UserActionType.b00_Control_Switch, HMI_Settings.UserRight ) )
                return;

            if ( HMI_Settings.isRegPass && !CommonUtils.CommonUtils.CanAction() )
                return;

            if ( MessageBox.Show( "Выполнить вывод?", "Попытка вывода АЧР", MessageBoxButtons.YesNo, MessageBoxIcon.Question )
                == DialogResult.No )
                return;

            /*ICommand cmd = */
            HMI_Settings.CONFIGURATION.ExecuteCommand( 0, uniDev, "AR0", new byte[] { }, this );
        }
        /// <summary>
        /// Идентификатор блока
        /// </summary>
        public uint Guid { get { return uniDev; } }
    }
}
