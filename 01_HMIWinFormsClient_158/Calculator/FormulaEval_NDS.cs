using System;
using System.Globalization;
using InterfaceLibrary;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Calculator
{
    /// <summary>
    /// ��������� ��� �������� ���������� ���������� �������
    /// </summary>
    public struct RezFormulaEval
    {
        /// <summary>
        /// ������, ���������������� ������������ ������� 
        /// � ������� "fc.dev.group.var.bitmask"
        /// </summary>
        public string IdTagIE;
        /// <summary>
        /// ������-�������� ������� ������������� ��������
        /// </summary>
        public string CaptionIE;
        /// <summary>
        /// ������-�������� ����������� ������������� ��������
        /// </summary>
        public string DimIE;
        /// <summary>
        /// ��� ������� �������� - 
        /// ��������� ������� � ���� bool-��������
        /// </summary>
        public object Value;
    }

    /// <summary>
    /// class FormulaEval
    /// �����, �������������� ����������� �������� ���������� ���� ��� 
    /// ����������������� �� �������� �������
    /// </summary>	
    public class FormulaEvalNds
    {
        private readonly string sourceFormula;
        public ITag LinkVariableNewDs; // �������� � ���������� � ����� DS
        private int flFc;
        private int flDevice;
        private int flGroup;
        private int flVariable;
        private string flBitMsk = string.Empty;

        public RezFormulaEval RezFormulaEval;

        #region ������ ���
        /// <summary>
        /// ������ ������� �������� 
        /// ��� ������� ��������� �� ��������� ����. ����
        /// </summary>
        /// <param name="strtagident"></param>
        /// <param name="valTag"></param>
        /// <returns></returns>
        public delegate bool ChangeValFormTI( string strtagident, object valTag, TypeOfTag type );

        /// <summary>
        /// ������ ������� ������� 
        /// �� ��������� ���������� ����������� �������
        /// </summary>
        public event ChangeValFormTI OnChangeValFormTI;
        #endregion

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="aTag"></param>
        /// <param name="f"></param>
        /// <param name="aCaptionIE"></param>
        /// <param name="aDimIE"></param>
        /// <param name="tot"></param>
        /// <param name="toP"></param>
        public FormulaEvalNds( IConfiguration configuration, string aTag, string f, string aCaptionIE, string aDimIE )
        {
            this.sourceFormula = aTag;

            /*--------------------------------------------------------------------
			 *	������ ������ ������� �����:
			 * 0(FC.FD.FG.FV.FB)1(xxx)..n(xxx), ���
			 * ����� 0..n - ������ �����, ���. ����� ��������� � ������� �� ���������� ���� �����
			 * FC - ����� ��
			 * FD	- ����� ����������
			 * FG - ����� ������
			 * FV - ����� ����������
			 * FB - ������� �����
			 *--------------------------------------------------------------------*/
            /*
			 * ���������� ���������� ��������� ��� ��������� 
			 * �������� ���� �����.�����.�����
			 */
            try
            {
                //Regex re = new Regex(@"[\d]+\.[\d]+\.[\d]+.[\d]+.[\d]+");

                //Match m = re.Match(SourceFormula);

                //string[] stidt = m.Value.Split(new char[]{'.'});
                string[] res1 = this.sourceFormula.Split( new[] { '(', ')' } );
                string[] stidt = res1[1].Split( new[] { '.' } );

                this.flFc = int.Parse( stidt[0] );
                flDevice = int.Parse( stidt[1] );
                flGroup = int.Parse( stidt[2] );
                flVariable = int.Parse( stidt[3] );
                flBitMsk = stidt[4];

                if ( !TagControlSet( configuration, this.flFc, flDevice, flGroup, flVariable, flBitMsk, aCaptionIE, aDimIE ) )
                    TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( TraceEventType.Error,
                                                                          361,
                                                                          string.Format( "{0} : {1} : {2} : ������ ��������������� ���� :\n aTag = {3};\n f = {4};\n aCaptionIE = {5}.\n", DateTime.Now.ToString( ), "FormulaEval_NDS.cs", "FormulaEvalNDS()", aTag, f, aCaptionIE ) );
            }
            catch ( Exception ex )
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( ex );
            }
        }

        /// <summary>
        /// ����������� ��� ������ DataServer
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="aTag"></param>
        /// <param name="f"></param>
        /// <param name="aCaptionIE"></param>
        /// <param name="aDimIE"></param>
        /// <param name="tot"></param>
        /// <param name="toP"></param>
        public FormulaEvalNds( IConfiguration configuration, string aTag, string aCaptionIE, string aDimIE )
        {
            this.sourceFormula = aTag;

            /*--------------------------------------------------------------------
              *	������ ������ ������� �����:
              * 0(DS.DEVGUID.TAGGUID), ���
              * 
              * DS - ����� DataServer
              * DEVGUID	- ���� ����� ����������
              * TAGGUID - ���� ����� ����
              *--------------------------------------------------------------------*/
            /*
              * ���������� ���������� ��������� ��� ��������� 
              * �������� ���� �����.�����.�����
              */
            try
            {
                Regex re = new Regex( @"[\d]+\.[\d]+\.[\d]+" ); //.[\d]+.[\d]+;

                Match m = re.Match( this.sourceFormula );

                string[] stidt = m.Value.Split( new char[] { '.' } );
                uint flDS = uint.Parse( stidt[0] );
                uint flDevice = uint.Parse( stidt[1] );
                uint flTagGuid = uint.Parse( stidt[2] );

                if ( !TagControlSet( configuration, flDS, flDevice, flTagGuid, aCaptionIE, aDimIE ) ) TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( TraceEventType.Error, 427, string.Format( "{0} : {1} : {2} : ������ ��������������� ���� :\n aTag = {3};\n aCaptionIE = {4}.\n", DateTime.Now.ToString( ), "FormulaEval_NDS.cs", "FormulaEvalNDS()", aTag, aCaptionIE ) );
            }
            catch ( Exception ex )
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( ex );
            }
        }
        /// <summary>
        /// ������ � ����� 1
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="lFC"></param>
        /// <param name="lDevice"></param>
        /// <param name="lGroup"></param>
        /// <param name="lVariable"></param>
        /// <param name="lBitMsk"></param>
        /// <returns></returns>
        private bool TagControlSet( IConfiguration configuration, int lFC, int lDevice, int lGroup, int lVariable, string lBitMsk, string aCaptionIE, string aDimIE )
        {
            try
            {
                this.flFc = lFC;
                flDevice = lDevice;
                flGroup = lGroup;
                flVariable = lVariable;
                flBitMsk = lBitMsk;

                var tagguid = HMI_MT_Settings.HMI_Settings.CONFIGURATION.GetTagGUID( 0, "MOA_ECU", string.Format( "{0}.{1}", lVariable.ToString( CultureInfo.InvariantCulture ), lBitMsk ) );

                var guidevice = (uint)( lFC * 256 + lDevice );

                var tag = configuration.GetLink2Tag( 0, guidevice, tagguid ); //(uint)lDevice

                if ( tag == null ) // ��� �� ������
                    return false;

                this.LinkVariableNewDs = tag;

                // ��� ���� ���������� ����������� �����
                this.RezFormulaEval.CaptionIE = aCaptionIE;
                this.RezFormulaEval.DimIE = aDimIE; // ����������� ����������

                // ��������� ������ ���������������� ���
                this.RezFormulaEval.IdTagIE = string.Format( "{0}.{1}.{2}.{3}.{4}", lFC, lDevice, lGroup, lVariable, lBitMsk );
                this.LinkVariableNewDs.OnChangeValue += LinkVariableNewDsOnOnChangeValue;
            }
            catch ( Exception ex )
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( ex );
                return false;
            }

            return true;
        }
        /// <summary>
        /// ������ � ����� - ������� ��� ������ �������� (����-100)
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="lFC"></param>
        /// <param name="lDevice"></param>
        /// <param name="lGroup"></param>
        /// <param name="lVariable"></param>
        /// <param name="lBitMsk"></param>
        /// <returns></returns>
        private bool TagControlSet( IConfiguration configuration, uint lds, uint lDevice, uint tagGuid, string aCaptionIE, string aDimIE )
        {
            try
            {
                var tag = configuration.GetLink2Tag( 0, lDevice, tagGuid );

                if ( tag == null ) // ��� �� ������
                    return false;

                this.LinkVariableNewDs = tag;

                // ��� ���� ���������� ����������� �����
                this.RezFormulaEval.CaptionIE = aCaptionIE;
                this.RezFormulaEval.DimIE = aDimIE;

                // ��������� ������ ���������������� ���(��� ������ ������)
                //tRezFormulaEval.IdTagIE = string.Format("0.{0}.0.0.{1}", tag.Device.UniObjectGUID, tag.TagGUID.ToString());

                // ��������� ������ ���������������� ���(��� ����� ������)
                this.RezFormulaEval.IdTagIE = string.Format( "{0}.{1}.{2}", tag.Device.UniDS_GUID, tag.Device.UniObjectGUID, tag.TagGUID.ToString( CultureInfo.InvariantCulture ) );
                this.LinkVariableNewDs.OnChangeValue += LinkVariableNewDsOnOnChangeValue;

                return true;
            }
            catch ( Exception ex )
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( ex );
                return false;
            }

        }
        /// <summary>
        /// ������� ��������� �������� �������/����
        /// </summary>
        /// <param name="var">������ � ������� �������</param>
        /// <param name="type">��� �������</param>
        private void LinkVariableNewDsOnOnChangeValue( Tuple<string, byte[], DateTime, VarQualityNewDs> @var, TypeOfTag type )
        {
            if ( OnChangeValFormTI == null ) return;

            try
            {
                switch ( type )
                {
                        //case TypeOfTag.Combo: // ���������� ��� ������
                    case TypeOfTag.Discret: // ����������������� �� true/false
                        OnChangeValFormTI( this.RezFormulaEval.IdTagIE, Convert.ToBoolean( @var.Item1 ) ? 1 : 0, type );
                        break;
                        //case TypeOfTag.Analog: // ���������� ��� ������
                        //    OnChangeValFormTI( this.tRezFormulaEval.IdTagIE, Convert.ToSingle( @var.Item1 ) );
                        //    break;
                        //case TypeOfTag.DateTime: // ���������� ��� ������
                        //    OnChangeValFormTI( this.tRezFormulaEval.IdTagIE, DateTime.Parse( @var.Item1 ) );
                        //    break;
                    default: // ���������� ��� ������
                        OnChangeValFormTI( this.RezFormulaEval.IdTagIE, @var.Item1, type );
                        break;
                }
            }
            catch ( Exception exception )
            {
                TraceSourceLib.TraceSourceDiagMes.WriteDiagnosticMSG( exception );
            }
        }
    }
}