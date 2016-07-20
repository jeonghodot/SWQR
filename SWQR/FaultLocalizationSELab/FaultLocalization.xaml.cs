using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Windows.Forms;
using System.Data;
using Fault_Localization_SE_Lab.Utility;
using Fault_Localization_SE_Lab.Instrument;
using Fault_Localization_SE_Lab.Test;
using OfficeOpenXml;
using Controls;
using System.Collections;

namespace Fault_Localization_SE_Lab
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        DataSet dsTestCase = new DataSet();
        DataSet dsSourceCode = new DataSet();
        DataSet dsResult = new DataSet();
        DataSet PdsResult = new DataSet();

        DataSet dsDistinct = new DataSet();
        DataSet FdsDistinct = new DataSet();
        DataSet PdsDistinct = new DataSet();
        bool flag_firstrun = true;
        Dictionary<int, string> dicArgument = new Dictionary<int, string>();

        System.Windows.Forms.CheckBox cb_dgvMain = new System.Windows.Forms.CheckBox();
        int cnt_selectedTC = 0;
        int nScroll_dgvMain = 0;
        bool flag_loadTC = false;
        bool flag_cb_dgvMain = false;
        string SOURCE_CODE_TYPE = string.Empty;
        int failcount = 0;
        int passcount = 0;


        public MainWindow()
        {
            InitializeComponent();
            this.Left = 0;
            this.Top = 0;
            dgvMain.CurrentCellDirtyStateChanged += new EventHandler(dgvMain_CurrentCellDirtyStateChanged);

            dsSourceCode.Tables.Add("SourceCode");
            //dsFSourceCode.Tables.Add("FSourceCode");
            //dsPSourceCode.Tables.Add("PSourceCode");
            dsSourceCode.Tables["SourceCode"].Columns.Add("Line", typeof(Int32));
            dsSourceCode.Tables["SourceCode"].Columns.Add("SourceCode", typeof(string));


            TestInfo.strResultPath = Environment.CurrentDirectory + "\\Result\\";
            TestInfo.strCurrentDirectory = Environment.CurrentDirectory + "\\";
            TestInfo.strInstrumentEditor = "InstrumentEditor.cs";
            TestInfo.strCoveragefile = "file.coverage";
            TestInfo.strCloverfile = "clover.xml";
            TestInfo.strTestResult = "temp_result.txt";//"TestResult.txt";
            string now = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            //MainLogger = new Logger("MainLogger");
            Logger.CreateLogger("MainLogger");

            TestInfo.strResultPath = TestInfo.strResultPath + now + "\\";
            TestInfo.strCoveragefile = TestInfo.strResultPath + @"\" + TestInfo.strCoveragefile;
            TestInfo.strCloverfile = TestInfo.strResultPath + @"\" + TestInfo.strCloverfile;
            string PROCESSOR_ARCHITECTURE = System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            Logger.WriteLine("PROCESSOR_ARCHITECTURE : " + PROCESSOR_ARCHITECTURE);
            TestInfo.OS_ARCH_TYPE = IniFile.GetIniValue("VisualStudioPrompt", PROCESSOR_ARCHITECTURE, Environment.CurrentDirectory + @"\Setting.ini");
            TestInfo.CODE_COVERAGE = IniFile.GetIniValue("Coverage", "Method", Environment.CurrentDirectory + @"\Setting.ini");

            //debug + database
            TestInfo.AutoRun = IniFile.GetIniValue("AutoRun", "AutoRun", Environment.CurrentDirectory + @"\Setting.ini");
            TestInfo.SourceCode = IniFile.GetIniValue("AutoRun", "SourceCode", Environment.CurrentDirectory + @"\Setting.ini");
            TestInfo.TestSuite = IniFile.GetIniValue("AutoRun", "TestSuite", Environment.CurrentDirectory + @"\Setting.ini");
            TestInfo.strProgramFilename = IniFile.GetIniValue("Database", "ProgramFilename", Environment.CurrentDirectory + @"\Setting.ini");
            TestInfo.strFaultyVersion = IniFile.GetIniValue("Database", "FaultyVersion", Environment.CurrentDirectory + @"\Setting.ini");
            TestInfo.strAnswerSheet = IniFile.GetIniValue("Database", "AnswerSheet", Environment.CurrentDirectory + @"\Setting.ini");
            //TestInfo.strFaultyVersion = "v1";
            //TestInfo.strAnswerSheet = "C://SKKUFL//DB//03AnswerSheet//AnswerSheet_0315.xlsx";

            if (!string.IsNullOrEmpty(TestInfo.SourceCode))
            {
                tbSourceCode.Text = TestInfo.SourceCode;
            }

            if (!string.IsNullOrEmpty(TestInfo.TestSuite))
            {
                tbTestSuite.Text = TestInfo.TestSuite;
            }

            if (!string.IsNullOrEmpty(TestInfo.strProgramFilename))
            {
                tbAnswerSheet.Text = TestInfo.strAnswerSheet;
                chkAnswerSheet.IsChecked = true;
            }

            if (!string.IsNullOrEmpty(TestInfo.strProgramFilename))
            {
                tbDBFile.Text = TestInfo.strProgramFilename;
                chkDBFile.IsChecked = true;
            }

            if (!string.IsNullOrEmpty(TestInfo.strProgramFilename))
            {
                tbFaultyVer.Text = TestInfo.strFaultyVersion;
            }

            DataGridViewCheckBoxColumn CheckboxColumn = new DataGridViewCheckBoxColumn();
            CheckboxColumn.DisplayIndex = 0;
            CheckboxColumn.Width = 100;
            CheckboxColumn.Name = "Select";
            CheckboxColumn.HeaderText = "";
            dgvMain.Columns.Add(CheckboxColumn);

            ObservableNodeList itemSource = new ObservableNodeList();

            //실험 대상 기법1

            Node TARANTULA = new Node("TARANTULA");
            TARANTULA.IsSelected = true;
            itemSource.Add(TARANTULA);
            cmbAlgorithm.ItemsSource = itemSource;

            //Node AMPLE = new Node("AMPLE");
            //AMPLE.IsSelected = true;
            //itemSource.Add(AMPLE);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Jaccard = new Node("Jaccard");
            //Jaccard.IsSelected = true;
            //itemSource.Add(Jaccard);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Dice = new Node("Dice");
            //Dice.IsSelected = true;
            //itemSource.Add(Dice);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node CZEKANOWSKI = new Node("CZEKANOWSKI");
            //CZEKANOWSKI.IsSelected = true;
            //itemSource.Add(CZEKANOWSKI);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node _3WJACCARD = new Node("_3WJACCARD");
            //_3WJACCARD.IsSelected = true;
            //itemSource.Add(_3WJACCARD);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node NEIandLI = new Node("NEIandLI");
            //NEIandLI.IsSelected = true;
            //itemSource.Add(NEIandLI);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SOKALandSNEATH_1 = new Node("SOKALandSNEATH_1");
            //SOKALandSNEATH_1.IsSelected = true;
            //itemSource.Add(SOKALandSNEATH_1);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SOKALandMICHENER = new Node("SOKALandMICHENER");
            //SOKALandMICHENER.IsSelected = true;
            //itemSource.Add(SOKALandMICHENER);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SOKALandSNEATH2 = new Node("SOKALandSNEATH2");
            //SOKALandSNEATH2.IsSelected = true;
            //itemSource.Add(SOKALandSNEATH2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node ROGERandTANIMOTO = new Node("ROGERandTANIMOTO");
            //ROGERandTANIMOTO.IsSelected = true;
            //itemSource.Add(ROGERandTANIMOTO);
            //cmbAlgorithm.ItemsSource = itemSource;


            //Node FAITH = new Node("FAITH");
            //FAITH.IsSelected = true;
            //itemSource.Add(FAITH);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GOWERandLEGENDRE = new Node("GOWERandLEGENDRE");
            //GOWERandLEGENDRE.IsSelected = true;
            //itemSource.Add(GOWERandLEGENDRE);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node INTERSECTION = new Node("INTERSECTION");
            //INTERSECTION.IsSelected = true;
            //itemSource.Add(INTERSECTION);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node INNERPRODUCT = new Node("INNERPRODUCT");
            //INNERPRODUCT.IsSelected = true;
            //itemSource.Add(INNERPRODUCT);
            //cmbAlgorithm.ItemsSource = itemSource;


            //Node RUSSELLandRAO = new Node("RUSSELLandRAO");
            //RUSSELLandRAO.IsSelected = true;
            //itemSource.Add(RUSSELLandRAO);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node HAMMING = new Node("HAMMING");
            //HAMMING.IsSelected = true;
            //itemSource.Add(HAMMING);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node EUCLID = new Node("EUCLID");
            //EUCLID.IsSelected = true;
            //itemSource.Add(EUCLID);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SQUARED_EUCLID = new Node("SQUARED_EUCLID");
            //SQUARED_EUCLID.IsSelected = true;
            //itemSource.Add(SQUARED_EUCLID);
            //cmbAlgorithm.ItemsSource = itemSource;



            //Node CANBERRA = new Node("CANBERRA");
            //CANBERRA.IsSelected = true;
            //itemSource.Add(CANBERRA);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node MANHATTAN = new Node("MANHATTAN");
            //MANHATTAN.IsSelected = true;
            //itemSource.Add(MANHATTAN);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node MEAN_MANHATTAN = new Node("MEAN_MANHATTAN");
            //MEAN_MANHATTAN.IsSelected = true;
            //itemSource.Add(MEAN_MANHATTAN);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node CITYBLOCK = new Node("CITYBLOCK");
            //CITYBLOCK.IsSelected = true;
            //itemSource.Add(CITYBLOCK);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node MINKOWSK = new Node("MINKOWSK");
            //MINKOWSK.IsSelected = true;
            //itemSource.Add(MINKOWSK);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node VARI = new Node("VARI");
            //VARI.IsSelected = true;
            //itemSource.Add(VARI);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SIZEDIFFERENCE = new Node("SIZEDIFFERENCE");
            //SIZEDIFFERENCE.IsSelected = true;
            //itemSource.Add(SIZEDIFFERENCE);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SHAPEDIFFERENCE = new Node("SHAPEDIFFERENCE");
            //SHAPEDIFFERENCE.IsSelected = true;
            //itemSource.Add(SHAPEDIFFERENCE);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node PATTERNDIFFERENCE = new Node("PATTERNDIFFERENCE");
            //PATTERNDIFFERENCE.IsSelected = true;
            //itemSource.Add(PATTERNDIFFERENCE);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node LANCEandWILLIAMS = new Node("LANCEandWILLIAMS");
            //LANCEandWILLIAMS.IsSelected = true;
            //itemSource.Add(LANCEandWILLIAMS);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node BRAYandCURTIS = new Node("BRAYandCURTIS");
            //BRAYandCURTIS.IsSelected = true;
            //itemSource.Add(BRAYandCURTIS);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node HELLINGER = new Node("HELLINGER");
            //HELLINGER.IsSelected = true;
            //itemSource.Add(HELLINGER);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node CHORD = new Node("CHORD");
            //CHORD.IsSelected = true;
            //itemSource.Add(CHORD);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node COSINE = new Node("COSINE");
            //COSINE.IsSelected = true;
            //itemSource.Add(COSINE);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GILBERTandWELLS = new Node("GILBERTandWELLS");
            //GILBERTandWELLS.IsSelected = true;
            //itemSource.Add(GILBERTandWELLS);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node OCHIAI1 = new Node("OCHIAI1");
            //OCHIAI1.IsSelected = true;
            //itemSource.Add(OCHIAI1);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node FORBESI = new Node("FORBESI");
            //FORBESI.IsSelected = true;
            //itemSource.Add(FORBESI);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node FOSSUM = new Node("FOSSUM");
            //FOSSUM.IsSelected = true;
            //itemSource.Add(FOSSUM);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SORGENFREI = new Node("SORGENFREI");
            //SORGENFREI.IsSelected = true;
            //itemSource.Add(SORGENFREI);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node MOUNTFORD = new Node("MOUNTFORD");
            //MOUNTFORD.IsSelected = true;
            //itemSource.Add(MOUNTFORD);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node OTSUKA = new Node("OTSUKA");
            //OTSUKA.IsSelected = true;
            //itemSource.Add(OTSUKA);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node MCCONNAUGHEY = new Node("MCCONNAUGHEY");
            //MCCONNAUGHEY.IsSelected = true;
            //itemSource.Add(MCCONNAUGHEY);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node TARWID = new Node("TARWID");
            //TARWID.IsSelected = true;
            //itemSource.Add(TARWID);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node KULCZYNSK2 = new Node("KULCZYNSK2");
            //KULCZYNSK2.IsSelected = true;
            //itemSource.Add(KULCZYNSK2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node DRIVERandKROEBER = new Node("DRIVERandKROEBER");
            //DRIVERandKROEBER.IsSelected = true;
            //itemSource.Add(DRIVERandKROEBER);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node JOHNSON = new Node("JOHNSON");
            //JOHNSON.IsSelected = true;
            //itemSource.Add(JOHNSON);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node DENNIS = new Node("DENNIS");
            //DENNIS.IsSelected = true;
            //itemSource.Add(DENNIS);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SIMPSON = new Node("SIMPSON");
            //SIMPSON.IsSelected = true;
            //itemSource.Add(SIMPSON);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node BRAUNandBANQUET = new Node("BRAUNandBANQUET");
            //BRAUNandBANQUET.IsSelected = true;
            //itemSource.Add(BRAUNandBANQUET);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node FAGERandMCGOWAN = new Node("FAGERandMCGOWAN");
            //FAGERandMCGOWAN.IsSelected = true;
            //itemSource.Add(FAGERandMCGOWAN);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node FORBES2 = new Node("FORBES2");
            //FORBES2.IsSelected = true;
            //itemSource.Add(FORBES2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SOKALandSNEATH4 = new Node("SOKALandSNEATH4");
            //SOKALandSNEATH4.IsSelected = true;
            //itemSource.Add(SOKALandSNEATH4);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GOWER = new Node("GOWER");
            //GOWER.IsSelected = true;
            //itemSource.Add(GOWER);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node PEARSON1 = new Node("PEARSON1");
            //PEARSON1.IsSelected = true;
            //itemSource.Add(PEARSON1);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node PEARSON2 = new Node("PEARSON2");
            //PEARSON2.IsSelected = true;
            //itemSource.Add(PEARSON2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node PEARSON3 = new Node("PEARSON3");
            //PEARSON3.IsSelected = true;
            //itemSource.Add(PEARSON3);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node PEARSONandHERON1 = new Node("PEARSONandHERON1");
            //PEARSONandHERON1.IsSelected = true;
            //itemSource.Add(PEARSONandHERON1);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node PEARSONandHERON2 = new Node("PEARSONandHERON2");
            //PEARSONandHERON2.IsSelected = true;
            //itemSource.Add(PEARSONandHERON2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SOKALandSNEATH3 = new Node("SOKALandSNEATH3");
            //SOKALandSNEATH3.IsSelected = true;
            //itemSource.Add(SOKALandSNEATH3);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SOKALandSNEATH5 = new Node("SOKALandSNEATH5");
            //SOKALandSNEATH5.IsSelected = true;
            //itemSource.Add(SOKALandSNEATH5);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node COLE = new Node("COLE");
            //COLE.IsSelected = true;
            //itemSource.Add(COLE);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node STILES = new Node("STILES");
            //STILES.IsSelected = true;
            //itemSource.Add(STILES);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node OCHIAI2 = new Node("OCHIAI2");
            //OCHIAI2.IsSelected = true;
            //itemSource.Add(OCHIAI2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node YULEQ = new Node("YULEQ");
            //YULEQ.IsSelected = true;
            //itemSource.Add(YULEQ);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node D_YULEQ = new Node("D_YULEQ");
            //D_YULEQ.IsSelected = true;
            //itemSource.Add(D_YULEQ);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node YULEw = new Node("YULEw");
            //YULEw.IsSelected = true;
            //itemSource.Add(YULEw);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node KULCZYNSKI1 = new Node("KULCZYNSKI1");
            //KULCZYNSKI1.IsSelected = true;
            //itemSource.Add(KULCZYNSKI1);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node TANIMOTO = new Node("TANIMOTO");
            //TANIMOTO.IsSelected = true;
            //itemSource.Add(TANIMOTO);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node DISPERSON = new Node("DISPERSON");
            //DISPERSON.IsSelected = true;
            //itemSource.Add(DISPERSON);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node HAMANN = new Node("HAMANN");
            //HAMANN.IsSelected = true;
            //itemSource.Add(HAMANN);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node MICHAEL = new Node("MICHAEL");
            //MICHAEL.IsSelected = true;
            //itemSource.Add(MICHAEL);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GOODMANandKRUSKAL = new Node("GOODMANandKRUSKAL");
            //GOODMANandKRUSKAL.IsSelected = true;
            //itemSource.Add(GOODMANandKRUSKAL);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node ANDERBERG = new Node("ANDERBERG");
            //ANDERBERG.IsSelected = true;
            //itemSource.Add(ANDERBERG);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node BARONI_URBANIandBUSER1 = new Node("BARONI_URBANIandBUSER1");
            //BARONI_URBANIandBUSER1.IsSelected = true;
            //itemSource.Add(BARONI_URBANIandBUSER1);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node BARONI_URBANIandBUSER2 = new Node("BARONI_URBANIandBUSER2");
            //BARONI_URBANIandBUSER2.IsSelected = true;
            //itemSource.Add(BARONI_URBANIandBUSER2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node PEIRCE = new Node("PEIRCE");
            //PEIRCE.IsSelected = true;
            //itemSource.Add(PEIRCE);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node EYRAUD = new Node("EYRAUD");
            //EYRAUD.IsSelected = true;
            //itemSource.Add(EYRAUD);
            //cmbAlgorithm.ItemsSource = itemSource;


            ///////////////////////////////////////////////////////////////////////////////////////


            //Node Naish = new Node("Naish");
            //Naish.IsSelected = true;
            //itemSource.Add(Naish);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GP08 = new Node("GP08");
            //GP08.IsSelected = true;
            //itemSource.Add(GP08);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GP10 = new Node("GP10");
            //GP10.IsSelected = true;
            //itemSource.Add(GP10);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GP11 = new Node("GP11");
            //GP11.IsSelected = true;
            //itemSource.Add(GP11);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GP13 = new Node("GP13");
            //GP13.IsSelected = true;
            //itemSource.Add(GP13);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GP20 = new Node("GP20");
            //GP20.IsSelected = true;
            //itemSource.Add(GP20);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GP26 = new Node("GP26");
            //GP26.IsSelected = true;
            //itemSource.Add(GP26);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SEM1 = new Node("SEM1");
            //SEM1.IsSelected = true;
            //itemSource.Add(SEM1);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SEM2 = new Node("SEM2");
            //SEM2.IsSelected = true;
            //itemSource.Add(SEM2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SEM3 = new Node("SEM3");
            //SEM3.IsSelected = true;
            //itemSource.Add(SEM3);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Wong2 = new Node("Wong2");
            //Wong2.IsSelected = true;
            //itemSource.Add(Wong2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Wong3 = new Node("Wong3");
            //Wong3.IsSelected = true;
            //itemSource.Add(Wong3);
            //cmbAlgorithm.ItemsSource = itemSource;


            ///////////////////////////////////////////////////////////////////////////////////////
















            //Node Tarantula = new Node("Tarantula");
            //Tarantula.IsSelected = true;
            //itemSource.Add(Tarantula);

            //Node AMPLE = new Node("AMPLE");
            //AMPLE.IsSelected = true;
            //itemSource.Add(AMPLE);

            //Node Jaccard = new Node("Jaccard");
            //Jaccard.IsSelected = true;
            //itemSource.Add(Jaccard);

            //Node Ochiai = new Node("Ochiai");
            //Ochiai.IsSelected = true;
            //itemSource.Add(Ochiai);

            //Node Heuristic3 = new Node("Heuristic3_c_");
            //Heuristic3.IsSelected = false;
            //itemSource.Add(Heuristic3);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Hybrid = new Node("Hybrid");
            //Hybrid.IsSelected = false;
            //itemSource.Add(Hybrid);
            //cmbAlgorithm.ItemsSource = itemSource;




            //Node Zoltar = new Node("Zoltar");
            //Zoltar.IsSelected = true;
            //itemSource.Add(Zoltar);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Kulczynski2 = new Node("Kulczynski2");
            //Kulczynski2.IsSelected = true;
            //itemSource.Add(Kulczynski2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Anderberg = new Node("Anderberg");
            //Anderberg.IsSelected = true;
            //itemSource.Add(Anderberg);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node M2 = new Node("M2");
            //M2.IsSelected = true;
            //itemSource.Add(M2);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Dice = new Node("Dice");
            //Dice.IsSelected = true;
            //itemSource.Add(Dice);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node PS = new Node("PS");
            //PS.IsSelected = false;
            //itemSource.Add(PS);
            //cmbAlgorithm.ItemsSource = itemSource;




            //Node SorensenDice = new Node("SorensenDice");
            //SorensenDice.IsSelected = true;
            //itemSource.Add(SorensenDice);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Wong1 = new Node("Wong1");
            //Wong1.IsSelected = true;
            //itemSource.Add(Wong1);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node SimpleMatching = new Node("SimpleMatching");
            //SimpleMatching.IsSelected = true;
            //itemSource.Add(SimpleMatching);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Sokal = new Node("Sokal");
            //Sokal.IsSelected = true;
            //itemSource.Add(Sokal);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node RogersTanimoto = new Node("RogersTanimoto");
            //RogersTanimoto.IsSelected = true;
            //itemSource.Add(RogersTanimoto);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Goodman = new Node("Goodman");
            //Goodman.IsSelected = true;
            //itemSource.Add(Goodman);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Hammingetc = new Node("Hammingetc");
            //Hammingetc.IsSelected = true;
            //itemSource.Add(Hammingetc);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Euclid = new Node("Euclid");
            //Euclid.IsSelected = true;
            //itemSource.Add(Euclid);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node M1 = new Node("M1");
            //M1.IsSelected = true;
            //itemSource.Add(M1);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Hamann = new Node("Hamann");
            //Hamann.IsSelected = true;
            //itemSource.Add(Hamann);
            //cmbAlgorithm.ItemsSource = itemSource;



            //Node RussellandRao = new Node("RussellandRao");
            //RussellandRao.IsSelected = true;
            //itemSource.Add(RussellandRao);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Cohen = new Node("Cohen");
            //Cohen.IsSelected = true;
            //itemSource.Add(Cohen);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node GeometricMean = new Node("GeometricMean");
            //GeometricMean.IsSelected = true;
            //itemSource.Add(GeometricMean);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node HarmonicMean = new Node("HarmonicMean");
            //HarmonicMean.IsSelected = true;
            //itemSource.Add(HarmonicMean);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node ArithmeticMean = new Node("ArithmeticMean");
            //ArithmeticMean.IsSelected = true;
            //itemSource.Add(ArithmeticMean);
            //cmbAlgorithm.ItemsSource = itemSource;

            //Node Rogot1 = new Node("Rogot1");
            //Rogot1.IsSelected = true;
            //itemSource.Add(Rogot1);
            //cmbAlgorithm.ItemsSource = itemSource;







            this.Title = this.Title + " " + IniFile.GetIniValue("Info", "Version", Environment.CurrentDirectory + @"\Setting.ini");

            //dgvSourceCode.AutoGenerateColumns = false;

            if (TestInfo.AutoRun == "True")
            {
                if (chkDBFile.IsChecked == true)
                {
                    //Start Test
                    Logger.WriteLine("Auto Run Started(Database) >> " + TestInfo.strProgramFilename + " " + TestInfo.strFaultyVersion + " " + TestInfo.strAnswerSheet);
                    StartTest();
                    Logger.WriteLine("Auto Run Done(Database) >> " + TestInfo.strProgramFilename + " " + TestInfo.strFaultyVersion + " " + TestInfo.strAnswerSheet);

                }
                else
                {
                    //Load and Select TC and Start Test
                    Logger.WriteLine("Auto Run Started(SourceCode) >> " + TestInfo.SourceCode + " " + TestInfo.TestSuite);
                    LoadTestSuite();
                    SelectAllTC();
                    SourCodeType();
                    StartTest();
                    Logger.WriteLine("Auto Run Done(SourceCode) >> " + TestInfo.SourceCode + " " + TestInfo.TestSuite);
                }
            }

        }

        public void SourCodeType()
        {
            SOURCE_CODE_TYPE = System.IO.Path.GetExtension(tbSourceCode.Text);

            TestInfo.SOURCE_CODE_TYPE = SOURCE_CODE_TYPE;
            if (SOURCE_CODE_TYPE.ToLower().Equals(".cs"))
            {
                TestInfo.strInstrumentEditor = "InstrumentEditor.cs";
            }
            else if (SOURCE_CODE_TYPE.ToLower().Equals(".c"))
            {
                TestInfo.strInstrumentEditor = "InstrumentEditor.h";
                TestInfo.strInstrumentEditorCpp = "InstrumentEditor.c";
            }
            else if (SOURCE_CODE_TYPE.ToLower().Equals(".cpp"))
            {
                TestInfo.strInstrumentEditor = "InstrumentEditor_cpp.h";
                TestInfo.strInstrumentEditorCpp = "InstrumentEditor_cpp.cpp";
            }
        }
        private void btnSourceCode_Click(object sender, RoutedEventArgs e)
        {
            tbSourceCode.Text = NativeMethod.getFileFullName("cs;c;cpp");
            SourCodeType();
            IniFile.SetIniValue("AutoRun", "SourceCode", tbSourceCode.Text, Environment.CurrentDirectory + @"\Setting.ini");
        }

        private void btnTestSuite_Click(object sender, RoutedEventArgs e)
        {
            tbTestSuite.Text = NativeMethod.getFileFullName("xls");
            IniFile.SetIniValue("AutoRun", "TestSuite", tbTestSuite.Text, Environment.CurrentDirectory + @"\Setting.ini");
        }

        public void LoadTestSuite()
        {
            try
            {
                if (string.IsNullOrEmpty(tbTestSuite.Text) || tbTestSuite.Text.Equals("Select file..."))
                {
                    System.Windows.Forms.MessageBox.Show("Select test suite");
                    return;
                }

                flag_loadTC = true;

                dsTestCase.Clear();
                dsSourceCode.Clear();
                //dsFSourceCode.Clear();
                //dsPSourceCode.Clear();
                dsDistinct.Clear();
                int col_TC_ID = dsSourceCode.Tables["SourceCode"].Columns["SourceCode"].Ordinal + 1;
                for (int i = col_TC_ID; i < dsSourceCode.Tables["SourceCode"].Columns.Count; i++)
                {
                    dsSourceCode.Tables["SourceCode"].Columns.RemoveAt(col_TC_ID);

                }

                //int Fcol_TC_ID = dsFSourceCode.Tables["FSourceCode"].Columns["SourceCode"].Ordinal + 1;
                //for (int i = Fcol_TC_ID; i < dsFSourceCode.Tables["FSourceCode"].Columns.Count; i++)
                //{

                //    dsFSourceCode.Tables["FSourceCode"].Columns.RemoveAt(Fcol_TC_ID);

                //}

                //int Pcol_TC_ID = dsPSourceCode.Tables["PSourceCode"].Columns["SourceCode"].Ordinal + 1;
                //for (int i = Pcol_TC_ID; i < dsPSourceCode.Tables["PSourceCode"].Columns.Count; i++)
                //{


                //    dsPSourceCode.Tables["PSourceCode"].Columns.RemoveAt(Pcol_TC_ID);
                //}
                /*
                                int Fcol_TC_ID = dsFSourceCode.Tables["FSourceCode"].Columns["FSourceCode"].Ordinal + 1;
                                for (int i = Fcol_TC_ID; i < dsFSourceCode.Tables["FSourceCode"].Columns.Count; i++)
                                {
                                    dsFSourceCode.Tables["FSourceCode"].Columns.RemoveAt(Fcol_TC_ID);
                                }

                                int Pcol_TC_ID = dsPSourceCode.Tables["PSourceCode"].Columns["PSourceCode"].Ordinal + 1;
                                for (int i = Pcol_TC_ID; i < dsPSourceCode.Tables["PSourceCode"].Columns.Count; i++)
                                {
                                    dsPSourceCode.Tables["PSourceCode"].Columns.RemoveAt(Pcol_TC_ID);
                                }
                */
                dsTestCase = NativeMethod.GetDataFromExcel(tbTestSuite.Text);
                tbTotalTC.Text = dsTestCase.Tables[0].Rows.Count.ToString();
                dgvMain.DataSource = dsTestCase.Tables[0];


                for (int i = 0; i < dgvMain.Columns.Count; i++)
                {
                    dgvMain.Columns[i].Width = 70;

                    if (dgvMain.Columns[i].Name.Equals("Expected value") || dgvMain.Columns[i].Name.Equals("Actual value"))
                        dgvMain.Columns[i].Width = 100;
                }

                TestInfo.bInstrument = false;
                TestCaseTabItem.IsSelected = true;

            }
            catch (Exception ex)
            {
                Logger.WriteLine("ex", "btnLoadTestSuite_Click exception : " + ex.ToString());
                // System.Windows.Forms.MessageBox.Show("btnLoadTestSuite_Click exception : " + ex.ToString());

            }

        }

        private void btnLoadTestSuite_Click(object sender, RoutedEventArgs e)
        {
            LoadTestSuite();
        }

        public void InstrumentSourceCode(string fileExtension)
        {
            Instrumentor inst = new Instrumentor();
            string strFileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(tbSourceCode.Text);

            string dirName = string.Empty;
            dirName = System.IO.Path.GetDirectoryName(tbSourceCode.Text);
            try
            {
                if (fileExtension.Equals(".cs"))
                {
                    TestInfo.strOriginalSourceFile = TestInfo.strResultPath + @"\" + strFileNameWithoutExtension + ".cs";
                    TestInfo.strInstrumentedBinaryFile = TestInfo.strResultPath + @"\" + strFileNameWithoutExtension + "_inst" + ".exe";

                    Directory.CreateDirectory(TestInfo.strResultPath);
                    //NativeMethod.CopyFolder(dirName, TestInfo.strResultPath);

                    File.Copy(tbSourceCode.Text, TestInfo.strOriginalSourceFile, true);
                    //File.Copy(Environment.CurrentDirectory + @"\Tools\" + TestInfo.strInstrumentEditor, TestInfo.strResultPath + TestInfo.strInstrumentEditor, true);

                    TestInfo.strInstrumentedSourceFile = inst.GenInstrumentedCode(TestInfo.strOriginalSourceFile, dsSourceCode,  fileExtension);

                    inst.GetReflectionInfo(TestInfo.strOriginalSourceFile);
                }
                else if (fileExtension.Equals(".cpp") || fileExtension.Equals(".c"))
                {
                    TestInfo.strOriginalSourceFile = TestInfo.strResultPath + @"\" + strFileNameWithoutExtension + fileExtension;
                    TestInfo.strInstrumentedBinaryFile = TestInfo.strResultPath + @"\" + strFileNameWithoutExtension + "_inst" + ".exe";

                    Directory.CreateDirectory(TestInfo.strResultPath);
                    NativeMethod.CopyFolder(dirName, TestInfo.strResultPath);

                    //File.Copy(tbSourceCode.Text, TestInfo.strOriginalSourceFile, true);
                    File.Copy(Environment.CurrentDirectory + @"\Tools\" + TestInfo.strInstrumentEditor, TestInfo.strResultPath + TestInfo.strInstrumentEditor, true);
                    File.Copy(Environment.CurrentDirectory + @"\Tools\" + TestInfo.strInstrumentEditorCpp, TestInfo.strResultPath + TestInfo.strInstrumentEditorCpp, true);

                    TestInfo.strInstrumentedSourceFile = inst.GenInstrumentedCode(TestInfo.strOriginalSourceFile, dsSourceCode, fileExtension);

                    //inst.GetReflectionInfo(TestInfo.strOriginalSourceFile);
                }

                TestInfo.bInstrument = true;
            }
            catch (Exception ex)
            {
                //  System.Windows.Forms.MessageBox.Show("InstrumentSourceCode Exception : " + ex.ToString());
            }
        }

        public bool RunTestCase(int index, string TC_ID, DataSet ds, string table_name)
        {
            Logger.WriteLine("[TC_ID] : " + TC_ID);

            string arguments = string.Empty;

            try
            {
                dsSourceCode.Tables["SourceCode"].Columns.Add(TC_ID, typeof(string));
                //dsFSourceCode.Tables["FSourceCode"].Columns.Add(TC_ID, typeof(string));
                //dsPSourceCode.Tables["PSourceCode"].Columns.Add(TC_ID, typeof(string));
                //DataGridViewTextBoxColumn subTitleColumn = new DataGridViewTextBoxColumn();

                //dsSourceCode.auto
                //subTitleColumn.HeaderText = TC_ID;
                //subTitleColumn.MinimumWidth = 60;
                //subTitleColumn.FillWeight = 1;
                //dsSourceCode.Tables["SourceCode"].Columns.Add(subTitleColumn);

            }
            catch (System.Exception ex)
            {

            }

            int col = ds.Tables[table_name].Columns[TC_ID].Ordinal;

            try
            {
                try
                {
                    if (File.Exists(TestInfo.strResultPath + @"temp_result.txt"))
                        File.Delete(TestInfo.strResultPath + @"temp_result.txt");

                    if (File.Exists(TestInfo.strCoveragefile))
                        File.Delete(TestInfo.strCoveragefile);

                    if (File.Exists(TestInfo.strCloverfile))
                        File.Delete(TestInfo.strCloverfile);

                }
                catch (System.Exception ex)
                {
                    //System.Windows.Forms.MessageBox.Show(ex.ToString());
                }

                int sleepCnt = 0;
                if (flag_firstrun)
                {
                    flag_firstrun = false;

                    StreamWriter file = new StreamWriter(TestInfo.strResultPath + @"\information.txt");
                    file.WriteLine(tbSourceCode.Text);
                    file.WriteLine(tbTestSuite.Text);
                    file.Close();

                    //Instrument
                    if (SOURCE_CODE_TYPE.ToLower().Equals(".cs"))
                    {
                        //NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strCurrentDirectory + @"\Tools\csc.exe " + @"""", @"/debug " + TestInfo.strInstrumentedSourceFile + @" " + TestInfo.strResultPath + TestInfo.strInstrumentEditor, TestInfo.strResultPath);
                        NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"csc.exe /debug:full " + @"""" + TestInfo.strInstrumentedSourceFile + @"""", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);
                    }
                    else if (SOURCE_CODE_TYPE.ToLower().Equals(".c") || SOURCE_CODE_TYPE.ToLower().Equals(".cpp"))
                    {
                        if (TestInfo.CODE_COVERAGE.Equals("VisualCoverage"))
                        {
                            //link + profile
                            NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"cl.exe /Zi " + @"""" + TestInfo.strInstrumentedSourceFile + @"""" + @" /link /profile", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);

                        }
                        else if (TestInfo.CODE_COVERAGE.Equals("CodeCoverage"))
                        {
                            //debug
                            NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"cl.exe /Zi " + @"""" + TestInfo.strInstrumentedSourceFile + @"""", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);

                        }
                        //NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"cl.exe /Zi " + @"""" + TestInfo.strInstrumentedSourceFile + @"""" + @" """ + TestInfo.strResultPath + TestInfo.strInstrumentEditorCpp + @"""" + @" /link /profile", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);
                        //NativeMethod.RunCommandLine(TestInfo.strCurrentDirectory + @"\Tools\cl.exe", @"/I ""C:\Program Files\Microsoft Visual Studio 10.0\VC\include"" /Zi " + TestInfo.strInstrumentedSourceFile + @" /link /LIBPATH:" + TestInfo.strCurrentDirectory + @"\Tools\lib /profile", TestInfo.strResultPath);     
                    }

                    sleepCnt = 0;
                    while (true)
                    {
                        if (sleepCnt > 5)
                            break;

                        if (File.Exists(TestInfo.strInstrumentedBinaryFile))
                            break;

                        if (SOURCE_CODE_TYPE.ToLower().Equals(".cs"))
                        {
                            //NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strCurrentDirectory + @"\Tools\csc.exe " + @"""", @"/debug " + TestInfo.strInstrumentedSourceFile + @" " + TestInfo.strResultPath + TestInfo.strInstrumentEditor, TestInfo.strResultPath);
                            NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"csc /debug " + @"""" + TestInfo.strInstrumentedSourceFile + @"""", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);
                        }
                        else if (SOURCE_CODE_TYPE.ToLower().Equals(".c") || SOURCE_CODE_TYPE.ToLower().Equals(".cpp"))
                        {
                            if (TestInfo.CODE_COVERAGE.Equals("VisualCoverage"))
                            {
                                //link + profile
                                NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"cl.exe /Zi " + @"""" + TestInfo.strInstrumentedSourceFile + @"""" + @" /link /profile", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);

                            }
                            else if (TestInfo.CODE_COVERAGE.Equals("CodeCoverage"))
                            {
                                //debug
                                NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"cl.exe /Zi " + @"""" + TestInfo.strInstrumentedSourceFile + @"""", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);

                            }
                        }

                        System.Threading.Thread.Sleep(1000);
                        sleepCnt++;
                    }

                    if (!File.Exists(TestInfo.strInstrumentedBinaryFile))
                    {
                        System.Windows.Forms.MessageBox.Show(TestInfo.strInstrumentedBinaryFile + " is not created");
                        return false;
                    }

                    if (TestInfo.CODE_COVERAGE.Equals("VisualCoverage"))
                    {
                        NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strCurrentDirectory + @"\Tools\vsinstr.exe" + @"""" + " " + @"/coverage " + @"""" + TestInfo.strInstrumentedBinaryFile + @"""", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);
                    }
                    //NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strCurrentDirectory + @"\Tools\vsperfcmd.exe" + @"""" + " " + @"/start:Coverage /Output:" + @"""" + TestInfo.strCoveragefile + @"""", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);

                    //NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\vsinstr.exe" + @"""", @"/coverage " + @"""" + TestInfo.strInstrumentedBinaryFile + @"""", TestInfo.strResultPath);

                }

                //argument injection is required              
                foreach (int key in dicArgument.Keys)
                {
                    arguments += dicArgument[key] + " ";
                }

                int LastIdxSpace = arguments.LastIndexOf(" ");
                arguments = arguments.Remove(LastIdxSpace, 1);
                ds.Tables[table_name].Rows[0][col] = arguments;


                if (TestInfo.CODE_COVERAGE.Equals("VisualCoverage"))
                {
                    NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\vsperfcmd.exe" + @"""", @"/start:Coverage /Output:" + @"""" + TestInfo.strCoveragefile + @"""", TestInfo.strResultPath);
                    NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strInstrumentedBinaryFile + @"""" + " " + arguments + " > temp_result.txt", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);
                    //NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strInstrumentedBinaryFile + @"""" + " " + arguments, TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);

                    NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\vsperfcmd.exe" + @"""", @"/Shutdown", TestInfo.strResultPath);
                    //NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strCurrentDirectory + @"\Tools\vsperfcmd.exe" + @"""" + " " + @"/Shutdown", TestInfo.OS_ARCH_TYPE,TestInfo.strResultPath);

                }
                else if (TestInfo.CODE_COVERAGE.Equals("CodeCoverage"))
                {
                    NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strInstrumentedBinaryFile + @"""" + " " + arguments + " > temp_result.txt", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);
                    NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strCurrentDirectory + @"\Tools\Dynamic Code Coverage Tools\CodeCoverage.exe" + @"""" + @" collect /output:" + @"""" + TestInfo.strCoveragefile + @"""" + @" " + @"""" + TestInfo.strInstrumentedBinaryFile + @"""" + " " + arguments, TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);
                    //NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\Dynamic Code Coverage Tools\CodeCoverage.exe" + @"""", @"collect /output:" + @"""" + TestInfo.strCoveragefile + @"""" + @" " + @"""" + TestInfo.strInstrumentedBinaryFile + @"""" + " " + arguments , TestInfo.strResultPath);
                }

                if (!File.Exists(TestInfo.strResultPath + TestInfo.strTestResult))
                {
                    //System.Windows.Forms.MessageBox.Show(TestInfo.strResultPath + TestInfo.strTestResult + " is not created");
                    Logger.WriteLine("[ERROR] " + TestInfo.strResultPath + TestInfo.strTestResult + " is not created");
                    dgvMain.Rows[index].Cells["Reason"].Value = (TestInfo.strResultPath + TestInfo.strTestResult + " is not created").ToString();
                    //return false;
                }

                sleepCnt = 0;
                while (true)
                {
                    if (sleepCnt > 5)
                        break;

                    if (File.Exists(TestInfo.strCoveragefile))
                        break;
                    System.Threading.Thread.Sleep(1000);
                    sleepCnt++;
                    Logger.WriteLine("strCoveragefile_cnt : " + sleepCnt.ToString());
                }

                if (TestInfo.CODE_COVERAGE.Equals("VisualCoverage"))
                {
                    //NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strCurrentDirectory + @"\Tools\visualcoverage.exe" + @"""" + " " + @"--input " + @"""" + TestInfo.strCoveragefile + @"""" + " --clover " + @"""" + TestInfo.strCloverfile + @"""", TestInfo.OS_ARCH_TYPE,TestInfo.strResultPath);
                    NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\visualcoverage.exe" + @"""", @"--input " + @"""" + TestInfo.strCoveragefile + @"""" + " --clover " + @"""" + TestInfo.strCloverfile + @"""", TestInfo.strResultPath);
                }
                else if (TestInfo.CODE_COVERAGE.Equals("CodeCoverage"))
                {
                    NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\Dynamic Code Coverage Tools\CodeCoverage.exe" + @"""", @"analyze /output:" + @"""" + TestInfo.strCloverfile + @"""" + @" " + @"""" + TestInfo.strCoveragefile + @"""", TestInfo.strResultPath);
                }

                sleepCnt = 0;
                while (true)
                {
                    if (sleepCnt > 5)
                        break;

                    if (File.Exists(TestInfo.strCloverfile))
                    {
                        if (sleepCnt > 0)
                            Logger.WriteLine("Data was recovered");
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                    if (TestInfo.CODE_COVERAGE.Equals("VisualCoverage"))
                    {
                        NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\vsperfcmd.exe" + @"""", @"/start:Coverage /Output:" + @"""" + TestInfo.strCoveragefile + @"""", TestInfo.strResultPath);
                        NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strInstrumentedBinaryFile + @"""" + " " + arguments + " > temp_result.txt", TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);
                        NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\vsperfcmd.exe" + @"""", @"/Shutdown", TestInfo.strResultPath);
                        NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\visualcoverage.exe" + @"""", @"--input " + @"""" + TestInfo.strCoveragefile + @"""" + " --clover " + @"""" + TestInfo.strCloverfile + @"""", TestInfo.strResultPath);
                    }
                    else if (TestInfo.CODE_COVERAGE.Equals("CodeCoverage"))
                    {
                        NativeMethod.RunCommandLineWIthVisualStudioPrompt(@"""" + TestInfo.strCurrentDirectory + @"\Tools\Dynamic Code Coverage Tools\CodeCoverage.exe" + @"""" + @" collect /output:" + @"""" + TestInfo.strCoveragefile + @"""" + @" " + @"""" + TestInfo.strInstrumentedBinaryFile + @"""" + " " + arguments, TestInfo.OS_ARCH_TYPE, TestInfo.strResultPath);
                        NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\Dynamic Code Coverage Tools\CodeCoverage.exe" + @"""", @"analyze /output:" + @"""" + TestInfo.strCloverfile + @"""" + @" " + @"""" + TestInfo.strCoveragefile + @"""", TestInfo.strResultPath);
                    }

                    sleepCnt++;
                    Logger.WriteLine("strCloverfile_cnt : " + sleepCnt.ToString());
                }

                if (!File.Exists(TestInfo.strCloverfile))
                {
                    //System.Windows.Forms.MessageBox.Show(TestInfo.strCloverfile + " is not created");
                    Logger.WriteLine("[ERROR] " + TestInfo.strCloverfile + " is not created");
                    dgvMain.Rows[index].Cells["Reason"].Value = (TestInfo.strCloverfile + " is not created").ToString();
                    //return false;
                }
                //NativeMethod.RunCommandLine(@"""" + TestInfo.strInstrumentedBinaryFile + @"""", arguments, TestInfo.strResultPath);
                //NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\vsperfcmd.exe" + @"""", @"/Shutdown", TestInfo.strResultPath);
                //NativeMethod.RunCommandLine(@"""" + TestInfo.strCurrentDirectory + @"\Tools\visualcoverage.exe" + @"""", @"--input " + @"""" + TestInfo.strCoveragefile + @"""" + " --clover " + @"""" + TestInfo.strCloverfile + @"""", TestInfo.strResultPath);

            }
            catch (System.Exception ex)
            {
                // System.Windows.Forms.MessageBox.Show(ex.ToString());
                return false;
            }

            return true;

        }

        public void StartTest()
        {
            try
            {
                try
                {
                    string algorithm = cmbAlgorithm.Text;
                    string[] split_al = algorithm.Split(',');

                    dsResult.Tables.Add("Result");
                    dsResult.Tables["Result"].Columns.Add("ProgramName", typeof(string));
                    dsResult.Tables["Result"].Columns.Add("FaultyVersion", typeof(string));
                    dsResult.Tables["Result"].Columns.Add("Type", typeof(string));



                    foreach (string str in split_al)
                    {
                        dsResult.Tables["Result"].Columns.Add(str + "_Best", typeof(string));
                        dsResult.Tables["Result"].Columns.Add(str + "_Worst", typeof(string));

                    }
                }
                catch (Exception)
                {

                }

                if ((bool)chkDBFile.IsChecked) //DB Use
                {
                    if (string.IsNullOrEmpty(tbDBFile.Text) || tbDBFile.Text.Equals("Select file..."))
                    {
                        System.Windows.Forms.MessageBox.Show("Select DBFile");
                        return;
                    }

                    dsTestCase.Clear();
                    dsSourceCode.Clear();
                    //dsFSourceCode.Clear();
                    //dsPSourceCode.Clear();
                    dsDistinct.Clear();
                    dsResult.Clear();
                    PdsResult.Clear();



                    dsSourceCode = NativeMethod.GetDataFromExcel(tbDBFile.Text, TestInfo.strFaultyVersion);
                    dsSourceCode.Tables[TestInfo.strFaultyVersion].TableName = "SourceCode";

                    //dsFSourceCode = NativeMethod.GetDataFromExcel(tbDBFile.Text, TestInfo.strFaultyVersion);
                    //dsFSourceCode.Tables[TestInfo.strFaultyVersion].TableName = "FSourceCode";

                    //dsPSourceCode = NativeMethod.GetDataFromExcel(tbDBFile.Text, TestInfo.strFaultyVersion);
                    //dsPSourceCode.Tables[TestInfo.strFaultyVersion].TableName = "PSourceCode";

                    try
                    {
                        ArrayList al = new ArrayList();
                        for (int col = 2; col < dsSourceCode.Tables["SourceCode"].Columns.Count; col++)
                        {
                            if (dsSourceCode.Tables["SourceCode"].Columns[col].ToString().Contains("TC"))
                                continue;
                            else
                                al.Add(dsSourceCode.Tables["SourceCode"].Columns[col].ToString());

                        }
                        for (int i = 0; i < al.Count; i++)
                        {
                            dsSourceCode.Tables["SourceCode"].Columns.Remove(al[i].ToString());
                        }


                        //for (int col = 2; col < dsFSourceCode.Tables["FSourceCode"].Columns.Count; col++)
                        //{
                        //    if (dsFSourceCode.Tables["FSourceCode"].Columns[col].ToString().Contains("TC"))
                        //        continue;
                        //    else
                        //        al.Add(dsFSourceCode.Tables["FSourceCode"].Columns[col].ToString());

                        //}
                        //for (int i = 0; i < al.Count; i++)
                        //{
                        //    dsFSourceCode.Tables["FSourceCode"].Columns.Remove(al[i].ToString());
                        //}



                        //for (int col = 2; col < dsPSourceCode.Tables["PSourceCode"].Columns.Count; col++)
                        //{
                        //    if (dsPSourceCode.Tables["PSourceCode"].Columns[col].ToString().Contains("TC"))
                        //        continue;
                        //    else
                        //        al.Add(dsPSourceCode.Tables["PSourceCode"].Columns[col].ToString());

                        //}
                        //for (int i = 0; i < al.Count; i++)
                        //{
                        //    dsPSourceCode.Tables["PSourceCode"].Columns.Remove(al[i].ToString());
                        //}



                    }
                    catch (Exception ex)
                    {

                    }

                }
                else //No DB
                {
                    if (string.IsNullOrEmpty(tbSourceCode.Text) || tbSourceCode.Text.Equals("Select file..."))
                    {
                        System.Windows.Forms.MessageBox.Show("Select source code");
                        return;
                    }

                    if (tbSelectedTC.Text.Equals("0"))
                    {
                        System.Windows.Forms.MessageBox.Show("Select TC");
                        return;
                    }

                    string arg_value = string.Empty;
                    int cnt_running = 0;
                    int ndicIndex = 0;
                    flag_loadTC = false;

                    if (TestInfo.bInstrument == false)
                    {
                        SourCodeType();
                        InstrumentSourceCode(SOURCE_CODE_TYPE);
                    }

                    int nArgColumn = dgvMain.Columns["arg1"].Index;
                    int row = dsSourceCode.Tables["SourceCode"].Rows.Count - 1;
                    int col = 0;
                    int Fcol = 0;
                    int Pcol = 0;

                    for (int i = 0; i < dgvMain.Rows.Count; i++)
                    {
                        dicArgument.Clear();
                        if (dgvMain.Rows[i].Cells["Select"].Value == null)
                        {
                            continue;
                        }

                        if ((bool)dgvMain.Rows[i].Cells["Select"].Value)
                        {
                            TestInfo.TC_ID = dgvMain.Rows[i].Cells["TC_ID"].Value.ToString();
                            for (int j = nArgColumn; j < dgvMain.ColumnCount; j++)
                            {
                                if (dgvMain.Columns[j].Name.Contains("arg"))
                                {
                                    arg_value = dgvMain.Rows[i].Cells[j].Value.ToString();

                                    if (!string.IsNullOrEmpty(arg_value) && !arg_value.Equals("-"))
                                    {
                                        dicArgument.Add(ndicIndex++, arg_value);
                                    }
                                }
                            }

                            //Run Current TC
                            if (!RunTestCase(i, TestInfo.TC_ID, dsSourceCode, "SourceCode"))
                                return;

                            //if (!RunTestCase(i, TestInfo.TC_ID, dsFSourceCode, "FSourceCode"))
                            //    return;

                            //if (!RunTestCase(i, TestInfo.TC_ID, dsPSourceCode, "PSourceCode"))
                            //    return;


                            if (File.Exists(TestInfo.strCloverfile))
                            {
                                XmlParser xpCoverage = new XmlParser(TestInfo.strCloverfile);
                                XmlParser FxpCoverage = new XmlParser(TestInfo.strCloverfile);
                                XmlParser PxpCoverage = new XmlParser(TestInfo.strCloverfile);

                                if (TestInfo.CODE_COVERAGE.Equals("VisualCoverage"))
                                {
                                    xpCoverage.SetCountInformation(dsSourceCode, TestInfo.TC_ID);
                                    //FxpCoverage.SetCountInformation(dsFSourceCode, TestInfo.TC_ID);
                                    //PxpCoverage.SetCountInformation(dsPSourceCode, TestInfo.TC_ID);
                                }
                                else if (TestInfo.CODE_COVERAGE.Equals("CodeCoverage"))
                                {
                                    xpCoverage.SetCountInformation_CodeCoverage(dsSourceCode, TestInfo.TC_ID);
                                    //xpCoverage.SetCountInformation_CodeCoverage(dsSourceCode, TestInfo.TC_ID);
                                    //FxpCoverage.SetCountInformation_CodeCoverage(dsFSourceCode, TestInfo.TC_ID); 
                                    //PxpCoverage.SetCountInformation_CodeCoverage(dsPSourceCode, TestInfo.TC_ID);
                                }
                            }

                            col = dsSourceCode.Tables["SourceCode"].Columns[TestInfo.TC_ID].Ordinal;
                            //Fcol = dsFSourceCode.Tables["FSourceCode"].Columns[TestInfo.TC_ID].Ordinal;
                            //Pcol = dsPSourceCode.Tables["PSourceCode"].Columns[TestInfo.TC_ID].Ordinal;

                            string result_value = NativeMethod.GetTestResultFromTXT(TestInfo.strResultPath + TestInfo.strTestResult);


                            result_value = result_value.Replace("\r", "");
                            result_value = @"""" + result_value + @"""";

                            /* for printtoken
                            //result_value = result_value.Replace("\r", "");
                            
                            //if (result_value.Length - result_value.LastIndexOf("\n") == 1 && result_value.LastIndexOf("\n") != -1)
                            //    result_value = result_value.Remove(result_value.LastIndexOf("\n"), 1);
                            //result_value = result_value.Replace("eof.\n", "eof.");
                            */
                            dgvMain.Rows[i].Cells["Actual value"].Value = result_value;

                            //if (dgvMain.Rows[i].Cells["Expected value"].Value.ToString().Length - dgvMain.Rows[i].Cells["Expected value"].Value.ToString().LastIndexOf("\n") == 1 && dgvMain.Rows[i].Cells["Expected value"].Value.ToString().LastIndexOf("\n") != -1)
                            //    dgvMain.Rows[i].Cells["Expected value"].Value = dgvMain.Rows[i].Cells["Expected value"].Value.ToString().Remove(dgvMain.Rows[i].Cells["Expected value"].Value.ToString().LastIndexOf("\n"), 1);

                            if (dgvMain.Rows[i].Cells["Actual value"].Value.ToString().Equals(dgvMain.Rows[i].Cells["Expected value"].Value.ToString()))
                            {
                                dgvMain.Rows[i].Cells["Test Result"].Value = "PASS";
                                dsSourceCode.Tables["SourceCode"].Rows[row][col] = "PASS";
                                //dsPSourceCode.Tables["PSourceCode"].Rows[row][Pcol] = "PASS";
                                //dsFSourceCode.Tables["FSourceCode"].Rows[row][Pcol] = "PASS";
                                //passcount++;
                            }
                            else
                            {
                                dgvMain.Rows[i].Cells["Test Result"].Value = "FAIL";
                                dsSourceCode.Tables["SourceCode"].Rows[row][col] = "FAIL";
                                //dsFSourceCode.Tables["FSourceCode"].Rows[row][Fcol] = "FAIL";
                                //dsPSourceCode.Tables["PSourceCode"].Rows[row][Pcol] = "FAIL";
                                //failcount++;
                            }
                            DataGridViewCheckBoxCell chkbox = new DataGridViewCheckBoxCell();
                            chkbox = (DataGridViewCheckBoxCell)dgvMain.Rows[i].Cells["Select"];
                            chkbox.Value = false;

                            cnt_running++;
                        }

                    }
                }

                //DataTable dtDistinct = new DataTable("SourceCode");
                //dtDistinct = MakeDistinctTable(dsSourceCode);

                //dsDistinct.Tables.Add(dtDistinct);
                //dsDistinct.Tables[0].TableName = "SourceCode";



                //int Ptable_last = dsPSourceCode.Tables.Count;
                //DataTable PdtDistinct = new DataTable("PSourceCode");
                //PdtDistinct = MakeDistinctTable2(dsPSourceCode);
                //dsPSourceCode.Tables["PSourceCode"].TableName = "SourceCode";
                //dsPSourceCode.Tables.Add(PdtDistinct);
                //dsPSourceCode.Tables[Ptable_last].TableName = "PSourceCode";
                //dsPSourceCode.Tables["SourceCode"].Clear();


                //int Ftable_last = dsFSourceCode.Tables.Count;
                //DataTable FdtDistinct = new DataTable("FSourceCode");
                //FdtDistinct = MakeDistinctTable3(dsFSourceCode);
                //dsFSourceCode.Tables["FSourceCode"].TableName = "SourceCode";
                //dsFSourceCode.Tables.Add(FdtDistinct);
                //dsFSourceCode.Tables[Ftable_last].TableName = "FSourceCode";
                //dsFSourceCode.Tables["SourceCode"].Clear();



                ComputeSuspiciousValue(dsSourceCode);
                ComputeRank(dsSourceCode, dsResult, "SourceCode_CodeCoverage");
                if (chkAnswerSheet.IsChecked == true)
                    ComputeExamScore(dsSourceCode, dsResult, "SourceCodeExam");


                //ComputeSuspiciousValue(dsDistinct);
                //ComputeRank(dsDistinct, dsResult, "Distinct_CodeCoverage");
                //if (chkAnswerSheet.IsChecked == true)
                //    ComputeExamScore(dsDistinct, dsResult, "DistinctExam");






                //DivideFP(dsFSourceCode, dsPSourceCode);

                //HammingDistance(dsFSourceCode, dsPSourceCode);

                //BrayCurtis(dsFSourceCode, dsPSourceCode);

                //Preprocess(dsFSourceCode, dsPSourceCode);


                //ComputeSuspiciousValue3(dsFSourceCode, dsPSourceCode);


                //ComputeRank2(dsPSourceCode, dsResult, "PSourceCode_CodeCoverage");
                //if (chkAnswerSheet.IsChecked == true)
                //    ComputeExamScore2(dsPSourceCode, dsResult, "PSourceCodeExam");



                try
                {
                    //dgvSourceCode.DataSource = null;
                    //dgvSourceCode.Columns.Clear();

                    //dgvSourceCode.DataSource = dsSourceCode.Tables["SourceCode"];

                    //dgvSourceCode.Columns["Line"].Width = 50;
                    //dgvSourceCode.Columns["SourceCode"].Width = 400;

                    //int col_TC_ID = dsSourceCode.Tables["SourceCode"].Columns["SourceCode"].Ordinal + 1;
                    //for (int i = col_TC_ID; i < dsSourceCode.Tables["SourceCode"].Columns.Count; i++)
                    //{
                    //    dgvSourceCode.Columns[i].Width = 60;
                    //    dgvSourceCode.Columns[i].FillWeight = 1;
                    //}

                    //ColoringBySuspicious(dsSourceCode, 0.8f, 1.0f);
                    //SourceCodeTabItem.IsSelected = true;
                    //chkShowTC.IsChecked = false;
                }
                catch (Exception error)
                {

                }


            }
            catch (Exception ex)
            {
                Logger.WriteLine("ex", "btnStartTest_Click exception : " + ex.ToString());
                // System.Windows.Forms.MessageBox.Show("btnStartTest_Click exception : " + ex.ToString());

            }
            finally
            {
                SaveTestResult();
                if (!TestInfo.AutoRun.Equals("True"))
                    System.Windows.Forms.MessageBox.Show("Test is completed");

                if (TestInfo.AutoRun.Equals("True"))
                {
                    //System.Windows.Forms.Application.Exit();
                    Environment.Exit(110);
                }
            }


        }

        private void btnStartTest_Click(object sender, RoutedEventArgs e)
        {
            StartTest();
        }


        DataTable GenerateTransposedTable(DataTable inputTable)
        {
            DataTable outputTable = new DataTable();

            // Add columns by looping rows

            // Header row's first column is same as in inputTable
            outputTable.Columns.Add(inputTable.Columns[0].ColumnName.ToString());

            // Header row's second column onwards, 'inputTable's first column taken
            foreach (DataRow inRow in inputTable.Rows)
            {
                string newColName = inRow[0].ToString();
                outputTable.Columns.Add(newColName);
            }

            // Add rows by looping columns        
            for (int rCount = 1; rCount <= inputTable.Columns.Count - 1; rCount++)
            {
                DataRow newRow = outputTable.NewRow();

                // First column is inputTable's Header row's second column
                newRow[0] = inputTable.Columns[rCount].ColumnName.ToString();
                for (int cCount = 0; cCount <= inputTable.Rows.Count - 1; cCount++)
                {
                    string colValue = inputTable.Rows[cCount][rCount].ToString();
                    newRow[cCount + 1] = colValue;
                }
                outputTable.Rows.Add(newRow);
            }

            return outputTable;
        }

        DataTable GetTransposeTable(DataTable dtOld)
        {
            DataTable dtNew = new DataTable();

            dtNew.Columns.Add(new DataColumn("0", typeof(string)));
            for (int i = 0; i < dtOld.Columns.Count; i++)
            {
                DataRow newRow = dtNew.NewRow();
                newRow[0] = dtOld.Columns[i].ColumnName;
                for (int j = 1; j <= dtOld.Rows.Count; j++)
                {
                    if (dtNew.Columns.Count < dtOld.Rows.Count + 1)
                        dtNew.Columns.Add(new DataColumn(j.ToString(), typeof(string)));
                    newRow[j] = dtOld.Rows[j - 1][i];
                }
                dtNew.Rows.Add(newRow);
            }
            return dtNew;
        }

        DataTable MakeDistinctTable(DataSet ds)
        {
            DataTable transposeTable = GenerateTransposedTable(ds.Tables["SourceCode"]); //GetTransposeTable(ds.Tables["SourceCode"]);
            //Distinct TC

            transposeTable.Columns.Remove("Line");
            transposeTable.Columns.Remove("0");

            var UniqueRows = transposeTable.AsEnumerable().Distinct(DataRowComparer.Default);
            DataTable dt2 = UniqueRows.CopyToDataTable();

            dt2.Columns.Add("Line");
            dt2.Columns.Add("0");

            dt2.Columns["0"].SetOrdinal(0);
            dt2.Columns["Line"].SetOrdinal(0);


            dt2.Rows[0]["Line"] = "SourceCode";

            DataTable transposeTable2 = GenerateTransposedTable(dt2);//GetTransposeTable(dt2);

            transposeTable2.Columns[0].ColumnName = "Line";
            transposeTable2.Columns[1].ColumnName = "SourceCode";

            for (int i = 2; i < transposeTable2.Columns.Count; i++)
            {
                transposeTable2.Columns[i].ColumnName = string.Format("TC_NEW_{0:0000}", i - 1);
            }

            ChangeColumnDataType(transposeTable2, "Line", typeof(int));
            transposeTable2.Columns["Line"].SetOrdinal(0);
            return transposeTable2;

        }

        DataTable MakeDistinctTable2(DataSet ds)
        {
            DataTable transposeTable = GenerateTransposedTable(ds.Tables["PSourceCode"]); //GetTransposeTable(ds.Tables["SourceCode"]);
            //Distinct TC

            transposeTable.Columns.Remove("Line");
            transposeTable.Columns.Remove("0");

            var UniqueRows = transposeTable.AsEnumerable().Distinct(DataRowComparer.Default);
            DataTable dt2 = UniqueRows.CopyToDataTable();

            dt2.Columns.Add("Line");
            dt2.Columns.Add("0");

            dt2.Columns["0"].SetOrdinal(0);
            dt2.Columns["Line"].SetOrdinal(0);


            dt2.Rows[0]["Line"] = "SourceCode";

            DataTable transposeTable2 = GenerateTransposedTable(dt2);//GetTransposeTable(dt2);

            transposeTable2.Columns[0].ColumnName = "Line";
            transposeTable2.Columns[1].ColumnName = "SourceCode";

            for (int i = 2; i < transposeTable2.Columns.Count; i++)
            {
                transposeTable2.Columns[i].ColumnName = string.Format("TC_NEW_{0:0000}", i - 1);
            }

            ChangeColumnDataType(transposeTable2, "Line", typeof(int));
            transposeTable2.Columns["Line"].SetOrdinal(0);
            return transposeTable2;

        }
        DataTable Sorting(DataSet ds)
        {
            DataTable transposeTable = GenerateTransposedTable(ds.Tables["PSourceCode"]); //GetTransposeTable(ds.Tables["SourceCode"]);
            //Distinct TC

            transposeTable.Columns.Remove("Line");
            transposeTable.Columns.Remove("0");


            transposeTable.Columns[0].ColumnName = "Hamming";

            DataView dt = new DataView(transposeTable);

            dt.Sort = "Hamming ASC";

            transposeTable = dt.ToTable();


            //foreach (DataRowView row in dt)
            //{
            //    Console.WriteLine(row["Hamming"]);
            //}

            transposeTable.Columns.Add("Line");
            transposeTable.Columns.Add("0");
            // transposeTable.Columns.Add("0");

            transposeTable.Columns["0"].SetOrdinal(0);
            transposeTable.Columns["Line"].SetOrdinal(0);



            transposeTable.Rows[0]["Line"] = "SourceCode";

            DataTable transposeTable2 = GenerateTransposedTable(transposeTable);//GetTransposeTable(dt2);

            transposeTable2.Columns[0].ColumnName = "Line";
            transposeTable2.Columns[1].ColumnName = "SourceCode";

            //for (int i = 2; i < transposeTable2.Columns.Count; i++)
            //{
            //    transposeTable2.Columns[i].ColumnName = string.Format("TC_NEW_{0:0000}", i - 1);
            //}

            ChangeColumnDataType(transposeTable2, "Line", typeof(int));
            transposeTable2.Columns["Line"].SetOrdinal(0);

            //RemoveHandler legacy column
            ds.Tables["PSourceCode"].Clear();
            ds.Tables["PSourceCode"].Reset();

            ds.Tables["PSourceCode"].Merge(transposeTable2);
            ds.Tables["PSourceCode"].Rows[1][0] = 1;
            return transposeTable2;

        }

        DataTable MakeDistinctTable3(DataSet ds)
        {
            DataTable transposeTable = GenerateTransposedTable(ds.Tables["FSourceCode"]); //GetTransposeTable(ds.Tables["SourceCode"]);
            //Distinct TC

            transposeTable.Columns.Remove("Line");
            transposeTable.Columns.Remove("0");

            var UniqueRows = transposeTable.AsEnumerable().Distinct(DataRowComparer.Default);
            DataTable dt2 = UniqueRows.CopyToDataTable();

            dt2.Columns.Add("Line");
            dt2.Columns.Add("0");

            dt2.Columns["0"].SetOrdinal(0);
            dt2.Columns["Line"].SetOrdinal(0);


            dt2.Rows[0]["Line"] = "SourceCode";

            DataTable transposeTable2 = GenerateTransposedTable(dt2);//GetTransposeTable(dt2);

            transposeTable2.Columns[0].ColumnName = "Line";
            transposeTable2.Columns[1].ColumnName = "SourceCode";

            for (int i = 2; i < transposeTable2.Columns.Count; i++)
            {
                transposeTable2.Columns[i].ColumnName = string.Format("TC_NEW_{0:0000}", i - 1);
            }

            ChangeColumnDataType(transposeTable2, "Line", typeof(int));
            transposeTable2.Columns["Line"].SetOrdinal(0);
            return transposeTable2;

        }



        bool ChangeColumnDataType(DataTable table, string columnname, Type newtype)
        {
            if (table.Columns.Contains(columnname) == false)
                return false;

            DataColumn column = table.Columns[columnname];
            if (column.DataType == newtype)
                return true;

            try
            {
                DataColumn newcolumn = new DataColumn("temperary", newtype);
                table.Columns.Add(newcolumn);
                foreach (DataRow row in table.Rows)
                {
                    try
                    {
                        row["temperary"] = Convert.ChangeType(row[columnname], newtype);
                    }
                    catch
                    {
                    }
                }
                table.Columns.Remove(columnname);
                newcolumn.ColumnName = columnname;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        void CopyColumns(DataTable source, DataTable dest, params string[] columns)
        {
            for (int row = 0; row < source.Rows.Count; row++)
            {
                foreach (string colname in columns)
                {
                    dest.Rows[row][colname] = source.Rows[row][colname];
                }
            }
        }

        void ComputeReadingCodeCoverage(DataSet ds, DataSet dsExam, string Type)
        {
            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {

                DataRow workRow = dsExam.Tables[0].NewRow();
                foreach (string al in split_al)
                {
                    string fld = al;//"Tarantula";
                    string fld_rank = al + "_Rank";
                    string query = string.Empty;

                    //Count lines more than 0

                    DataTable dtNumberOfLines = ds.Tables["SourceCode"].Select(fld + " > 0 ").CopyToDataTable();

                    int ReadingCodeCoverage = dtNumberOfLines.Rows.Count - 1;


                }
                dsExam.Tables[0].Rows.Add(workRow);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("ex", "ComputeRankAndExamScore exception : " + ex.ToString());
                //System.Windows.Forms.MessageBox.Show("ComputeRankAndExamScore exception : " + ex.ToString());

            }

        }

        void ComputeRank(DataSet ds, DataSet dsExam, string Type)
        {
            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {


                //Add READING CODE COVERAGE
                DataRow workRow = dsExam.Tables[0].NewRow();
                int nNumberOfLinesMoreThanZero = 0;

                string fld;
                foreach (string al in split_al)
                {
                    fld = al;//"Tarantula";

                    DataTable rankDt0 = (from data in ds.Tables["SourceCode"].AsEnumerable()
                                         select data).CopyToDataTable();

                    for (int i = 0; i < rankDt0.Rows.Count; i++)
                    {


                        if (rankDt0.Rows[i][fld].ToString() == null || rankDt0.Rows[i][fld].ToString() == "" || rankDt0.Rows[i][fld].ToString() == " ")
                        {
                            rankDt0.Rows[i][fld] = double.NegativeInfinity;
                        }
                        //string temp0 = rankDt0.Rows[i][fld].ToString();
                        //double temp02 = double.Parse(temp0);
                        //if (double.IsNaN(temp02))
                        //{
                        //    rankDt0.Rows[i][fld] = double.NegativeInfinity;

                        //}
                    }

                    ChangeColumnDataType(rankDt0, fld, typeof(double));

                    DataTable rankDt = (from data in rankDt0.AsEnumerable()
                                        orderby data.Field<double>(fld) descending
                                        select data).CopyToDataTable();




                    int col_idx_fld = ds.Tables["SourceCode"].Columns[al].Ordinal;

                    try
                    {
                        rankDt.Columns.Add(fld + "_Rank");
                    }
                    catch (Exception)
                    {
                    }

                    rankDt.Columns[fld + "_Rank"].SetOrdinal(col_idx_fld);

                    //Add READING CODE COVERAGE
                    nNumberOfLinesMoreThanZero = 0;
                    int rank = 1;
                    for (int i = 0; i < rankDt.Rows.Count - 1 - 1; i++)
                    {


                        if (rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                            rankDt.Rows[i][fld + "_Rank"] = rank;


                        if (rankDt.Rows[i][fld].ToString() != rankDt.Rows[i + 1][fld].ToString() && rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                            rank++;

                        if (rankDt.Rows[i][fld].ToString() != "0" && rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                        {
                            if (rankDt.Rows[i][fld].Equals(double.NegativeInfinity))

                            {

                            }
                            else
                            {
                                nNumberOfLinesMoreThanZero++;
                            }
                        }


                    }

                    string strProgramName = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strProgramFilename);
                    string strFaultyVersion = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strFaultyVersion);

                    workRow["ProgramName"] = strProgramName;
                    workRow["FaultyVersion"] = strFaultyVersion;
                    workRow["Type"] = Type;
                    double nScore = (double)nNumberOfLinesMoreThanZero * 100 / (double)(rankDt.Rows.Count - 2);
                    workRow[al + "_Best"] = Math.Round(nScore, 5);



                    //rankDt.Rows[rankDt.Rows.Count - 1][fld + "_Rank"] = rank;

                    ChangeColumnDataType(rankDt, "Line", typeof(int));
                    DataTable rankDt2 = (from data in rankDt.AsEnumerable()
                                         orderby data.Field<int>("Line") ascending
                                         select data).CopyToDataTable();

                    CopyColumns(rankDt2, ds.Tables["SourceCode"], fld + "_Rank");

                    rankDt.Clear();
                    rankDt2.Clear();
                }
                dsExam.Tables[0].Rows.Add(workRow);

            }
            catch (Exception ex)
            {
                Logger.WriteLine("ex", "ComputeRank : " + ex.ToString());
                // System.Windows.Forms.MessageBox.Show("ComputeRank exception : " + ex.ToString());

            }

        }

        void ComputeRank2(DataSet ds, DataSet dsExam, string Type)
        {
            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {


                //Add READING CODE COVERAGE
                DataRow workRow = dsExam.Tables[0].NewRow();
                int nNumberOfLinesMoreThanZero = 0;

                string fld;
                foreach (string al in split_al)
                {
                    fld = al;//"Tarantula";

                    DataTable rankDt0 = (from data in ds.Tables["PSourceCode"].AsEnumerable()
                                         select data).CopyToDataTable();

                    for (int i = 0; i < rankDt0.Rows.Count; i++)
                    {


                        if (rankDt0.Rows[i][fld].ToString() == null || rankDt0.Rows[i][fld].ToString() == "" || rankDt0.Rows[i][fld].ToString() == " ")
                        {
                            rankDt0.Rows[i][fld] = double.NegativeInfinity;
                        }
                        string temp = rankDt0.Rows[i][fld].ToString();
                        double temp2 = double.Parse(temp);
                        if (double.IsNaN(temp2))
                        {
                            rankDt0.Rows[i][fld] = double.NegativeInfinity;

                        }


                    }

                    ChangeColumnDataType(rankDt0, fld, typeof(double));

                    DataTable rankDt = (from data in rankDt0.AsEnumerable()
                                        orderby data.Field<Double>(fld) descending
                                        select data).CopyToDataTable();




                    int col_idx_fld = ds.Tables["PSourceCode"].Columns[al].Ordinal;

                    try
                    {
                        rankDt.Columns.Add(fld + "_Rank");
                    }
                    catch (Exception)
                    {
                    }

                    rankDt.Columns[fld + "_Rank"].SetOrdinal(col_idx_fld);

                    //Add READING CODE COVERAGE
                    nNumberOfLinesMoreThanZero = 0;
                    int rank = 1;
                    for (int i = 0; i < rankDt.Rows.Count - 1 - 1; i++)
                    {


                        if (rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                            rankDt.Rows[i][fld + "_Rank"] = rank;


                        if (rankDt.Rows[i][fld].ToString() != rankDt.Rows[i + 1][fld].ToString() && rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                            rank++;

                        if (rankDt.Rows[i][fld].ToString() != "0" && rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                        {
                            if (rankDt.Rows[i][fld].Equals(double.NegativeInfinity))
                            {

                            }
                            else
                            {
                                nNumberOfLinesMoreThanZero++;
                            }

                        }


                    }

                    string strProgramName = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strProgramFilename);
                    string strFaultyVersion = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strFaultyVersion);

                    workRow["ProgramName"] = strProgramName;
                    workRow["FaultyVersion"] = strFaultyVersion;
                    workRow["Type"] = Type;
                    double nScore = (double)nNumberOfLinesMoreThanZero * 100 / (double)(rankDt.Rows.Count - 2);
                    workRow[al + "_Best"] = Math.Round(nScore, 5);



                    //rankDt.Rows[rankDt.Rows.Count - 1][fld + "_Rank"] = rank;

                    ChangeColumnDataType(rankDt, "Line", typeof(int));
                    DataTable rankDt2 = (from data in rankDt.AsEnumerable()
                                         orderby data.Field<int>("Line") ascending
                                         select data).CopyToDataTable();

                    CopyColumns(rankDt2, ds.Tables["PSourceCode"], fld + "_Rank");

                    rankDt.Clear();
                    rankDt2.Clear();
                }
                dsExam.Tables[0].Rows.Add(workRow);

            }
            catch (Exception ex)
            {
                Logger.WriteLine("ex", "ComputeRank2 : " + ex.ToString());
                // System.Windows.Forms.MessageBox.Show("ComputeRank2 exception : " + ex.ToString());

            }

        }

        void ComputeRank0(DataSet ds, DataSet dsExam, string Type)
        {
            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {


                //Add READING CODE COVERAGE
                DataRow workRow = dsExam.Tables[0].NewRow();
                int nNumberOfLinesMoreThanZero = 0;

                string fld;
                foreach (string al in split_al)
                {
                    fld = al;//"Tarantula";

                    DataTable rankDt = (from data in ds.Tables["SourceCode"].AsEnumerable()
                                        orderby data.Field<double>(fld) descending
                                        select data).CopyToDataTable();

                    int col_idx_fld = ds.Tables["SourceCode"].Columns[al].Ordinal;

                    try
                    {
                        rankDt.Columns.Add(fld + "_Rank");
                    }
                    catch (Exception)
                    {
                    }

                    rankDt.Columns[fld + "_Rank"].SetOrdinal(col_idx_fld);

                    //Add READING CODE COVERAGE
                    nNumberOfLinesMoreThanZero = 0;
                    int rank = 1;


                    //for (int i = 0; i < rankDt.Rows.Count - 2; i++)
                    //{


                    //        rankDt.Rows[i][fld + "_Rank"] = rank;


                    //        if (rankDt.Rows[i][fld].ToString() != rankDt.Rows[i + 1][fld].ToString() && rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                    //        {
                    //            rank++;
                    //        }





                    //    if (rankDt.Rows[i][fld].ToString() != "0" && rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                    //    {
                    //        nNumberOfLinesMoreThanZero++;
                    //    }
                    //}




                    for (int i = 0; i < rankDt.Rows.Count - 2; i++)
                    {

                        if (rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                            rankDt.Rows[i][fld + "_Rank"] = rank;

                        if (rankDt.Rows[i][fld].ToString() != rankDt.Rows[i + 1][fld].ToString() && rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                            rank++;

                        if (rankDt.Rows[i][fld].ToString() != "0" && rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                        {
                            nNumberOfLinesMoreThanZero++;
                        }
                    }






                    string strProgramName = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strProgramFilename);
                    string strFaultyVersion = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strFaultyVersion);

                    workRow["ProgramName"] = strProgramName;
                    workRow["FaultyVersion"] = strFaultyVersion;
                    workRow["Type"] = Type;
                    double nScore = (double)nNumberOfLinesMoreThanZero * 100 / (double)(rankDt.Rows.Count - 2);
                    workRow[al + "_Best"] = Math.Round(nScore, 5);



                    //rankDt.Rows[rankDt.Rows.Count - 1][fld + "_Rank"] = rank;

                    ChangeColumnDataType(rankDt, "Line", typeof(int));
                    DataTable rankDt2 = (from data in rankDt.AsEnumerable()
                                         orderby data.Field<int>("Line") ascending
                                         select data).CopyToDataTable();

                    CopyColumns(rankDt2, ds.Tables["SourceCode"], fld + "_Rank");

                    rankDt.Clear();
                    rankDt2.Clear();
                }
                dsExam.Tables[0].Rows.Add(workRow);

            }
            catch (Exception ex)
            {
                Logger.WriteLine("ex", "ComputeRank : " + ex.ToString());
                // System.Windows.Forms.MessageBox.Show("ComputeRank exception : " + ex.ToString());

            }

        }


        void ComputeRank3(DataSet ds, DataSet dsExam, string Type)
        {
            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {


                //Add READING CODE COVERAGE
                DataRow workRow = dsExam.Tables[0].NewRow();
                int nNumberOfLinesMoreThanZero = 0;

                string fld;
                foreach (string al in split_al)
                {
                    fld = al;//"Tarantula";

                    //         Console.WriteLine(al);

                    DataTable rankDt = (from data in ds.Tables["PSourceCode"].AsEnumerable()
                                        orderby data.Field<string>(fld) descending
                                        select data).CopyToDataTable();

                    int col_idx_fld = ds.Tables["PSourceCode"].Columns[al].Ordinal;

                    try
                    {
                        rankDt.Columns.Add(fld + "_Rank");
                    }
                    catch (Exception)
                    {
                    }

                    rankDt.Columns[fld + "_Rank"].SetOrdinal(col_idx_fld);

                    //Add READING CODE COVERAGE
                    nNumberOfLinesMoreThanZero = 0;
                    int rank = 1;
                    for (int i = 0; i < rankDt.Rows.Count - 1 - 1; i++)
                    {
                        if (rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                            rankDt.Rows[i][fld + "_Rank"] = rank;

                        if (rankDt.Rows[i][fld].ToString() != rankDt.Rows[i + 1][fld].ToString() && rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                            rank++;

                        if (rankDt.Rows[i][fld].ToString() != "0" && rankDt.Rows[i + 1][fld].ToString() != "" && rankDt.Rows[i + 1][fld].ToString() != " ")
                        {
                            nNumberOfLinesMoreThanZero++;
                        }
                    }

                    string strProgramName = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strProgramFilename);
                    string strFaultyVersion = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strFaultyVersion);
                    //  strProgramName = "printtokens2";
                    workRow["ProgramName"] = strProgramName;
                    workRow["FaultyVersion"] = strFaultyVersion;
                    workRow["Type"] = Type;
                    double nScore = (double)nNumberOfLinesMoreThanZero * 100 / (double)(rankDt.Rows.Count - 2);
                    workRow[al + "_Best"] = Math.Round(nScore, 5);



                    //rankDt.Rows[rankDt.Rows.Count - 1][fld + "_Rank"] = rank;

                    ChangeColumnDataType(rankDt, "Line", typeof(int));
                    DataTable rankDt2 = (from data in rankDt.AsEnumerable()
                                         orderby data.Field<int>("Line") ascending
                                         select data).CopyToDataTable();

                    CopyColumns(rankDt2, ds.Tables["PSourceCode"], fld + "_Rank");

                    rankDt.Clear();
                    rankDt2.Clear();
                }
                dsExam.Tables[0].Rows.Add(workRow);

            }
            catch (Exception ex)
            {
                Logger.WriteLine("ex", "ComputeRank : " + ex.ToString());
                // System.Windows.Forms.MessageBox.Show("ComputeRank exception : " + ex.ToString());

            }

        }



        void ComputeExamScore(DataSet ds, DataSet dsExam, string Type)
        {
            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {

                DataRow workRow = dsExam.Tables[0].NewRow();
                foreach (string al in split_al)
                {
                    string fld = al;//"Tarantula";
                    string fld_rank = al + "_Rank";
                    string query = string.Empty;

                    //Answersheet and check TBest and TWorst
                    DataSet dsAnswerSheet = new DataSet();
                    dsAnswerSheet = NativeMethod.GetDataFromExcel(tbAnswerSheet.Text);

                    string strProgramName = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strProgramFilename);
                    //query = @"(ProgramName = 'printtokens') AND (FaultyVersion = 'v1')";
                    query = @"(ProgramName = '" + strProgramName + @"') AND (FaultyVersion = '" + TestInfo.strFaultyVersion + @"')";
                    DataTable dtAnswerSheet = dsAnswerSheet.Tables[0].Select(query).CopyToDataTable();

                    string strFaultyLine = dtAnswerSheet.Rows[0]["FaultyLine"].ToString();
                    DataTable dtRank = ds.Tables["SourceCode"].Select("Line = " + strFaultyLine).CopyToDataTable();

                    string strRank = dtRank.Rows[0][fld_rank].ToString();
                    DataTable dtNumberOfRank = ds.Tables["SourceCode"].Select(fld_rank + " = " + "'" + strRank + "'").CopyToDataTable();

                    int TBest = int.Parse(strRank);
                    int TWorst = int.Parse(strRank) + dtNumberOfRank.Rows.Count - 1;
                    int TotalLines = int.Parse(ds.Tables["SourceCode"].Rows[ds.Tables["SourceCode"].Rows.Count - 1][0].ToString());

                    double TBestExamScore = (double)TBest * 100 / (double)TotalLines;
                    double TWorstExamScore = (double)TWorst * 100 / (double)TotalLines;


                    Logger.WriteLine2(strProgramName+ "/"+ TestInfo.strFaultyVersion+"/"+TBest.ToString() + "/" +TWorst.ToString()+"/"+ TotalLines);
                        



                    string strFaultyVersion = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strFaultyVersion);

                    workRow["ProgramName"] = strProgramName;
                    workRow["FaultyVersion"] = strFaultyVersion;
                    workRow["Type"] = Type;
                    workRow[al + "_Best"] = Math.Round(TBestExamScore, 5);
                    workRow[al + "_Worst"] = Math.Round(TWorstExamScore, 5);

                }
                dsExam.Tables[0].Rows.Add(workRow);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("ex", "ComputeRankAndExamScore exception : " + ex.ToString());
                // System.Windows.Forms.MessageBox.Show("ComputeRankAndExamScore exception : " + ex.ToString());

            }



        }


        void ComputeExamScore2(DataSet ds, DataSet dsExam, string Type)
        {
            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {

                DataRow workRow = dsExam.Tables[0].NewRow();
                foreach (string al in split_al)
                {

                    //    string al = "Reinforce";
                    string fld = al;//"Tarantula";
                    string fld_rank = al + "_Rank";
                    string query = string.Empty;

                    //Answersheet and check TBest and TWorst
                    DataSet dsAnswerSheet = new DataSet();
                    dsAnswerSheet = NativeMethod.GetDataFromExcel(tbAnswerSheet.Text);

                    string strProgramName = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strProgramFilename);
                    //     Console.WriteLine(strProgramName);

                    //query = @"(ProgramName = 'printtokens') AND (FaultyVersion = 'v1')";
                    query = @"(ProgramName = '" + strProgramName + @"') AND (FaultyVersion = '" + TestInfo.strFaultyVersion + @"')";


                    DataTable dtAnswerSheet = dsAnswerSheet.Tables[0].Select(query).CopyToDataTable();

                    string strFaultyLine = dtAnswerSheet.Rows[0]["FaultyLine"].ToString();
                    DataTable dtRank = ds.Tables["PSourceCode"].Select("Line = " + strFaultyLine).CopyToDataTable();

                    string strRank = dtRank.Rows[0][fld_rank].ToString();
                    DataTable dtNumberOfRank = ds.Tables["PSourceCode"].Select(fld_rank + " = " + "'" + strRank + "'").CopyToDataTable();

                    int TBest = int.Parse(strRank);
                    int TWorst = int.Parse(strRank) + dtNumberOfRank.Rows.Count - 1;
                    int TotalLines = int.Parse(ds.Tables["PSourceCode"].Rows[ds.Tables["PSourceCode"].Rows.Count - 1][0].ToString());

                    double TBestExamScore = (double)TBest * 100 / (double)TotalLines;
                    double TWorstExamScore = (double)TWorst * 100 / (double)TotalLines;

                    string strFaultyVersion = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strFaultyVersion);

                    workRow["ProgramName"] = strProgramName;
                    workRow["FaultyVersion"] = strFaultyVersion;
                    workRow["Type"] = Type;
                    workRow[al + "_Best"] = Math.Round(TBestExamScore, 5);
                    workRow[al + "_Worst"] = Math.Round(TWorstExamScore, 5);

                }
                dsExam.Tables[0].Rows.Add(workRow);
            }
            catch (Exception ex)
            {
                Logger.WriteLine("ex", "ComputeRankAndExamScore exception : " + ex.ToString());
                //   System.Windows.Forms.MessageBox.Show("ComputeRankAndExamScore exception : " + ex.ToString());

            }
        }

        void Preprocess(DataSet dsF, DataSet dsP)
        {
            int Fcol_TC_ID = dsF.Tables["FSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Frow_last = dsF.Tables["FSourceCode"].Rows.Count;
            int Fcol_last = dsF.Tables["FSourceCode"].Columns.Count;

            int Pcol_TC_ID = dsP.Tables["PSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Prow_last = dsP.Tables["PSourceCode"].Rows.Count;
            int Pcol_last = dsP.Tables["PSourceCode"].Columns.Count;



            for (int Pcol = Pcol_TC_ID; Pcol < Pcol_last; Pcol++)

            {
                dsP.Tables["PSourceCode"].Columns[Pcol].ColumnName = string.Format("P{0:0000}", Pcol - 1);


            }

            for (int Fcol = Fcol_TC_ID; Fcol < Fcol_last; Fcol++)

            {

                dsF.Tables["FSourceCode"].Columns[Fcol].ColumnName = string.Format("F{0:0000}", Fcol - 1);//네이밍
            }


        }

        public double Sqrt(double formula)
        {
            // double SqrtValues = 0;
            if (formula > 0)
            {
                formula = Math.Sqrt(formula);
                //   Logger.WriteLine("formula1: " + formula);
            }
            else if (formula == 0)
            {
                formula = 0;
            }
            else if (formula < 0)
            {
                formula = -formula;
                formula = Math.Sqrt(formula);
                formula = -formula;
                //  Logger.WriteLine("formula2: " + formula);
            }
            return formula;

        }
        void ComputeSuspiciousValue3(DataSet dsF, DataSet dsP)
        {
            int Fcol_TC_ID = dsF.Tables["FSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Frow_last = dsF.Tables["FSourceCode"].Rows.Count;
            int Fcol_last = dsF.Tables["FSourceCode"].Columns.Count;

            int Pcol_TC_ID = dsP.Tables["PSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Prow_last = dsP.Tables["PSourceCode"].Rows.Count;
            int Pcol_last = dsP.Tables["PSourceCode"].Columns.Count;

            double d, b, c, a, blank = 0;
            int Pcol_last2 = Pcol_last;
            string result;
            string mark;

            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {

                dsP.Tables["PSourceCode"].Columns.Add("d");
                dsP.Tables["PSourceCode"].Columns.Add("b");
                dsP.Tables["PSourceCode"].Columns.Add("c");
                dsP.Tables["PSourceCode"].Columns.Add("a");
                dsP.Tables["PSourceCode"].Columns.Add("unnecessary");





                foreach (string al in split_al)
                {

                    dsP.Tables["PSourceCode"].Columns.Add(al, typeof(string));
                    dsP.Tables["PSourceCode"].Columns.Add(al + "_Rank", typeof(string));
                    dsP.Tables["PSourceCode"].Columns[al].ColumnName = al;
                    dsP.Tables["PSourceCode"].Columns[al + "_Rank"].ColumnName = al + "_Rank";

                }

            }
            catch (System.Exception ex)
            {

            }
            finally
            {

                dsP.Tables["PSourceCode"].Columns["d"].SetOrdinal(dsP.Tables["PSourceCode"].Columns.Count - 1);
                dsP.Tables["PSourceCode"].Columns["b"].SetOrdinal(dsP.Tables["PSourceCode"].Columns.Count - 1);
                dsP.Tables["PSourceCode"].Columns["c"].SetOrdinal(dsP.Tables["PSourceCode"].Columns.Count - 1);
                dsP.Tables["PSourceCode"].Columns["a"].SetOrdinal(dsP.Tables["PSourceCode"].Columns.Count - 1);
                dsP.Tables["PSourceCode"].Columns["unnecessary"].SetOrdinal(dsP.Tables["PSourceCode"].Columns.Count - 1);

                foreach (string al in split_al)
                {

                    dsP.Tables["PSourceCode"].Columns[al].SetOrdinal(dsP.Tables["PSourceCode"].Columns.Count - 1);
                    dsP.Tables["PSourceCode"].Columns[al + "_Rank"].SetOrdinal(dsP.Tables["PSourceCode"].Columns.Count - 1);


                }
            }
            double numerator, Ldenominator, Rdenominator, temp1, temp2, temp3;
            double folumla = 0.0f;
            double susp = 0.0f;
            double health = 0.0f;

            int counter;
            int Fcoltemp = 0;

            double TARANTULA = 0.0f;
            double AMPLE = 0.0f;
            double Jaccard = 0.0f;
            double Dice = 0.0f;
            double CZEKANOWSKI = 0.0f;
            double _3WJACCARD = 0.0f;
            double NEIandLI = 0.0f;
            double SOKALandSNEATH_1 = 0.0f;
            double SOKALandMICHENER = 0.0f;
            double SOKALandSNEATH2 = 0.0f;
            double ROGERandTANIMOTO = 0.0f;
            double FAITH = 0.0f;
            double GOWERandLEGENDRE = 0.0f;
            double INTERSECTION = 0.0f;
            double INNERPRODUCT = 0.0f;
            double RUSSELLandRAO = 0.0f;
            double HAMMING = 0.0f;
            double EUCLID = 0.0f;
            double SQUARED_EUCLID = 0.0f;
            double CANBERRA = 0.0f;
            double MANHATTAN = 0.0f;
            double MEAN_MANHATTAN = 0.0f;
            double CITYBLOCK = 0.0f;
            double MINKOWSK = 0.0f;
            double VARI = 0.0f;
            double SIZEDIFFERENCE = 0.0f;
            double SHAPEDIFFERENCE = 0.0f;
            double PATTERNDIFFERENCE = 0.0f;
            double LANCEandWILLIAMS = 0.0f;
            double BRAYandCURTIS = 0.0f;
            double HELLINGER = 0.0f;
            double CHORD = 0.0f;
            double COSINE = 0.0f;
            double GILBERTandWELLS = 0.0f;
            double OCHIAI1 = 0.0f;
            double FORBESI = 0.0f;
            double FOSSUM = 0.0f;
            double SORGENFREI = 0.0f;
            double MOUNTFORD = 0.0f;
            double OTSUKA = 0.0f;
            double MCCONNAUGHEY = 0.0f;
            double TARWID = 0.0f;
            double KULCZYNSK2 = 0.0f;
            double DRIVERandKROEBER = 0.0f;
            double JOHNSON = 0.0f;
            double DENNIS = 0.0f;
            double SIMPSON = 0.0f;
            double BRAUNandBANQUET = 0.0f;
            double FAGERandMCGOWAN = 0.0f;
            double FORBES2 = 0.0f;
            double SOKALandSNEATH4 = 0.0f;
            double GOWER = 0.0f;
            double PEARSON1 = 0.0f;
            double PEARSON2 = 0.0f;
            double PEARSON3 = 0.0f;
            double PEARSONandHERON1 = 0.0f;
            double PEARSONandHERON2 = 0.0f;
            double SOKALandSNEATH3 = 0.0f;
            double SOKALandSNEATH5 = 0.0f;
            double COLE = 0.0f;
            double STILES = 0.0f;
            double OCHIAI2 = 0.0f;
            double YULEQ = 0.0f;
            double D_YULEQ = 0.0f;
            double YULEw = 0.0f;
            double KULCZYNSKI1 = 0.0f;
            double TANIMOTO = 0.0f;
            double DISPERSON = 0.0f;
            double HAMANN = 0.0f;
            double MICHAEL = 0.0f;
            double GOODMANandKRUSKAL = 0.0f;
            double ANDERBERG = 0.0f;
            double BARONI_URBANIandBUSER1 = 0.0f;
            double BARONI_URBANIandBUSER2 = 0.0f;
            double PEIRCE = 0.0f;
            double EYRAUD = 0.0f;


            double TARANTULAfolumla = 0.0f;
            double AMPLEfolumla = 0.0f;
            double Jaccardfolumla = 0.0f;
            double Dicefolumla = 0.0f;
            double CZEKANOWSKIfolumla = 0.0f;
            double _3WJACCARDfolumla = 0.0f;
            double NEIandLIfolumla = 0.0f;
            double SOKALandSNEATH_1folumla = 0.0f;
            double SOKALandMICHENERfolumla = 0.0f;
            double SOKALandSNEATH2folumla = 0.0f;
            double ROGERandTANIMOTOfolumla = 0.0f;
            double FAITHfolumla = 0.0f;
            double GOWERandLEGENDREfolumla = 0.0f;
            double INTERSECTIONfolumla = 0.0f;
            double INNERPRODUCTfolumla = 0.0f;
            double RUSSELLandRAOfolumla = 0.0f;
            double HAMMINGfolumla = 0.0f;
            double EUCLIDfolumla = 0.0f;
            double SQUARED_EUCLIDfolumla = 0.0f;
            double CANBERRAfolumla = 0.0f;
            double MANHATTANfolumla = 0.0f;
            double MEAN_MANHATTANfolumla = 0.0f;
            double CITYBLOCKfolumla = 0.0f;
            double MINKOWSKfolumla = 0.0f;
            double VARIfolumla = 0.0f;
            double SIZEDIFFERENCEfolumla = 0.0f;
            double SHAPEDIFFERENCEfolumla = 0.0f;
            double PATTERNDIFFERENCEfolumla = 0.0f;
            double LANCEandWILLIAMSfolumla = 0.0f;
            double BRAYandCURTISfolumla = 0.0f;
            double HELLINGERfolumla = 0.0f;
            double CHORDfolumla = 0.0f;
            double COSINEfolumla = 0.0f;
            double GILBERTandWELLSfolumla = 0.0f;
            double OCHIAI1folumla = 0.0f;
            double FORBESIfolumla = 0.0f;
            double FOSSUMfolumla = 0.0f;
            double SORGENFREIfolumla = 0.0f;
            double MOUNTFORDfolumla = 0.0f;
            double OTSUKAfolumla = 0.0f;
            double MCCONNAUGHEYfolumla = 0.0f;
            double TARWIDfolumla = 0.0f;
            double KULCZYNSK2folumla = 0.0f;
            double DRIVERandKROEBERfolumla = 0.0f;
            double JOHNSONfolumla = 0.0f;
            double DENNISfolumla = 0.0f;
            double SIMPSONfolumla = 0.0f;
            double BRAUNandBANQUETfolumla = 0.0f;
            double FAGERandMCGOWANfolumla = 0.0f;
            double FORBES2folumla = 0.0f;
            double SOKALandSNEATH4folumla = 0.0f;
            double GOWERfolumla = 0.0f;
            double PEARSON1folumla = 0.0f;
            double PEARSON2folumla = 0.0f;
            double PEARSON3folumla = 0.0f;
            double PEARSONandHERON1folumla = 0.0f;
            double PEARSONandHERON2folumla = 0.0f;
            double SOKALandSNEATH3folumla = 0.0f;
            double SOKALandSNEATH5folumla = 0.0f;
            double COLEfolumla = 0.0f;
            double STILESfolumla = 0.0f;
            double OCHIAI2folumla = 0.0f;
            double YULEQfolumla = 0.0f;
            double D_YULEQfolumla = 0.0f;
            double YULEwfolumla = 0.0f;
            double KULCZYNSKI1folumla = 0.0f;
            double TANIMOTOfolumla = 0.0f;
            double DISPERSONfolumla = 0.0f;
            double HAMANNfolumla = 0.0f;
            double MICHAELfolumla = 0.0f;
            double GOODMANandKRUSKALfolumla = 0.0f;
            double ANDERBERGfolumla = 0.0f;
            double BARONI_URBANIandBUSER1folumla = 0.0f;
            double BARONI_URBANIandBUSER2folumla = 0.0f;
            double PEIRCEfolumla = 0.0f;
            double EYRAUDfolumla = 0.0f;



            for (int row = Prow_last - 2; row > 0; row--)
            {
                d = b = c = a = 0;
                numerator = 0;
                Ldenominator = 0;
                Rdenominator = 0;
                susp = 0.0f;
                health = 0.0f;
                temp1 = 0.0f;
                counter = 0;

                int PassTCcount = 0;


                double PTCPortion = 0.9;
                int PTCNumber = 7;

                // DPTC 설정
                //비율 for (int col = Pcol_last - 1; col > ((Pcol_last - 1) *PTCPortion); col--)
                //갯수 for (int col = Pcol_last - 1; col > ((Pcol_last - 1) - PTCNumber); col--)

                if (Pcol_last > 10)
                {
                    PassTCcount = 0;
                    for (int col = Pcol_last - 1; col > ((Pcol_last - 1) * 0.3); col--)
                    {
                        mark = dsP.Tables["PSourceCode"].Rows[row][col].ToString();
                        result = dsP.Tables["PSourceCode"].Rows[Prow_last - 1][col].ToString();

                        if (result.Equals("PASS"))
                        {
                            PassTCcount = PassTCcount + 1;
                            if (mark.Equals("1"))
                            { //●
                                c++;
                            }
                            else if (mark.Equals("") || mark.Equals("0"))
                            {
                                d++;
                            }

                        }
                    }


                }
                else
                {
                    PassTCcount = 0;
                    for (int col = Pcol_last - 1; col > 2; col--)
                    {
                        mark = dsP.Tables["PSourceCode"].Rows[row][col].ToString();
                        result = dsP.Tables["PSourceCode"].Rows[Prow_last - 1][col].ToString();

                        if (result.Equals("PASS"))
                        {
                            PassTCcount = PassTCcount + 1;
                            if (mark.Equals("1"))
                            { //●
                                c++;
                            }
                            else if (mark.Equals("") || mark.Equals("0"))
                            {
                                d++;
                            }

                        }

                    }



                }

                if (PassTCcount == d && c == 0)
                {
                    d = 0;
                }

                dsP.Tables["PSourceCode"].Rows[row]["d"] = d;
                //   dsP.Tables["PSourceCode"].Rows[row]["b"] = b;
                dsP.Tables["PSourceCode"].Rows[row]["c"] = c;
                //   dsP.Tables["PSourceCode"].Rows[row]["a"] = a;


                for (int Fcol = Fcol_TC_ID; Fcol < Fcol_last; Fcol++)
                {


                    string FailTC = string.Format("F{0:0000}", Fcol - 1);


                    if (dsF.Tables["FSourceCode"].Rows[row][FailTC].ToString().Equals("1"))
                    {
                        a = 1;
                        b = 0;

                    }
                    else
                    {
                        a = 0;
                        b = 1;
                    }



                    dsP.Tables["PSourceCode"].Rows[row]["a"] = a;
                    dsP.Tables["PSourceCode"].Rows[row]["b"] = b;


                    if (d == 0 && c == 0 && a == 0)
                    {
                        dsP.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;

                    }
                    else
                    {


                        foreach (string al in split_al)
                        {

                            if (al == "TARANTULA")
                            {
                                try
                                {
                                    if ((a + b) == 0)
                                        Ldenominator = numerator = 0.0f;
                                    else
                                        Ldenominator = numerator = (double)a / (a + b);
                                    if ((c + d) == 0)
                                        Rdenominator = 0.0f;
                                    else
                                        Rdenominator = (double)c / (c + d);

                                    if ((Ldenominator + Rdenominator) == 0)
                                        TARANTULAfolumla = 0.0f;
                                    else
                                        TARANTULAfolumla = (double)numerator / (Ldenominator + Rdenominator);


                                }
                                catch (System.Exception ex)
                                {
                                    TARANTULAfolumla = 0.0f;
                                }

                                finally
                                {
                                    TARANTULA += TARANTULAfolumla;
                                }
                            }


                            if (al == "AMPLE")
                            {
                                try
                                {
                                    if ((b + a) == 0)
                                        Ldenominator = 0.0f;
                                    else
                                        Ldenominator = (double)a / (b + a);
                                    if ((d + c) == 0)
                                        Rdenominator = 0.0f;
                                    else
                                        Rdenominator = (double)c / (d + c);

                                    AMPLEfolumla = Math.Abs(Ldenominator - Rdenominator);

                                }
                                catch (System.Exception ex)
                                {
                                    AMPLEfolumla = 0.0f;
                                }
                                finally
                                {
                                    AMPLE += AMPLEfolumla;
                                }
                            }


                            if (al == "Jaccard")
                            {

                                try
                                {
                                    if ((a + b + c) == 0)
                                        Jaccardfolumla = 0.0f;
                                    else
                                        Jaccardfolumla = a / (a + b + c);

                                }
                                catch (System.Exception ex)
                                {
                                    Jaccardfolumla = 0.0f;
                                }

                                finally
                                {
                                    Jaccard += Jaccardfolumla;
                                }


                            }
                            if (al == "Dice")
                            {

                                try
                                {
                                    if ((2 * a + b + c) == 0)
                                        Dicefolumla = 0.0f;
                                    else
                                        Dicefolumla = (2 * a) / (2 * a + b + c);

                                }
                                catch (System.Exception ex)
                                {
                                    Dicefolumla = 0.0f;
                                }

                                finally
                                {
                                    Dice += Dicefolumla;
                                }


                            }
                            if (al == "CZEKANOWSKI")
                            {

                                try
                                {
                                    if ((2 * a + b + c) == 0)
                                        CZEKANOWSKIfolumla = 0.0f;
                                    else
                                        CZEKANOWSKIfolumla = (2 * a) / (2 * a + b + c);

                                }
                                catch (System.Exception ex)
                                {
                                    CZEKANOWSKIfolumla = 0.0f;
                                }

                                finally
                                {
                                    CZEKANOWSKI += CZEKANOWSKIfolumla;
                                }


                            }
                            if (al == "_3WJACCARD")
                            {

                                try
                                {
                                    if ((3 * a + b + c) == 0)
                                        _3WJACCARDfolumla = 0.0f;
                                    else
                                        _3WJACCARDfolumla = (3 * a) / (3 * a + b + c);

                                }
                                catch (System.Exception ex)
                                {
                                    _3WJACCARDfolumla = 0.0f;
                                }

                                finally
                                {
                                    _3WJACCARD += _3WJACCARDfolumla;
                                }


                            }
                            if (al == "NEIandLI")
                            {

                                try
                                {
                                    if (((a + b) + (a + c)) == 0)
                                        NEIandLIfolumla = 0.0f;
                                    else
                                        NEIandLIfolumla = (2 * a) / ((a + b) + (a + c));

                                }
                                catch (System.Exception ex)
                                {
                                    NEIandLIfolumla = 0.0f;
                                }

                                finally
                                {
                                    NEIandLI += NEIandLIfolumla;
                                }


                            }
                            if (al == "SOKALandSNEATH_1")
                            {

                                try
                                {
                                    if ((a + 2 * b + 2 * c) == 0)
                                        SOKALandSNEATH_1folumla = 0.0f;
                                    else
                                        SOKALandSNEATH_1folumla = a / (a + 2 * b + 2 * c);

                                }
                                catch (System.Exception ex)
                                {
                                    SOKALandSNEATH_1folumla = 0.0f;
                                }

                                finally
                                {
                                    SOKALandSNEATH_1 += SOKALandSNEATH_1folumla;
                                }


                            }
                            if (al == "SOKALandMICHENER")
                            {

                                try
                                {
                                    if ((a + b + c + d) == 0)
                                        SOKALandMICHENERfolumla = 0.0f;
                                    else
                                        SOKALandMICHENERfolumla = (a + d) / (a + b + c + d);

                                }
                                catch (System.Exception ex)
                                {
                                    SOKALandMICHENERfolumla = 0.0f;
                                }

                                finally
                                {
                                    SOKALandMICHENER += SOKALandMICHENERfolumla;
                                }


                            }
                            if (al == "SOKALandSNEATH2")
                            {

                                try
                                {
                                    if ((2 * a + b + c + 2 * d) == 0)
                                        SOKALandSNEATH2folumla = 0.0f;
                                    else
                                        SOKALandSNEATH2folumla = (2 * (a + d)) / (2 * a + b + c + 2 * d);

                                }
                                catch (System.Exception ex)
                                {
                                    SOKALandSNEATH2folumla = 0.0f;
                                }

                                finally
                                {
                                    SOKALandSNEATH2 += SOKALandSNEATH2folumla;
                                }


                            }
                            if (al == "ROGERandTANIMOTO")
                            {

                                try
                                {
                                    if ((a + 2 * (b + c) + d) == 0)
                                        ROGERandTANIMOTOfolumla = 0.0f;
                                    else
                                        ROGERandTANIMOTOfolumla = (a + d) / (a + 2 * (b + c) + d);

                                }
                                catch (System.Exception ex)
                                {
                                    ROGERandTANIMOTOfolumla = 0.0f;
                                }

                                finally
                                {
                                    ROGERandTANIMOTO += ROGERandTANIMOTOfolumla;
                                }


                            }
                            if (al == "FAITH")
                            {

                                try
                                {
                                    if ((a + b + c + d) == 0)
                                        FAITHfolumla = 0.0f;
                                    else
                                        FAITHfolumla = (a + (0.5 * d)) / (a + b + c + d);

                                }
                                catch (System.Exception ex)
                                {
                                    FAITHfolumla = 0.0f;
                                }

                                finally
                                {
                                    FAITH += FAITHfolumla;
                                }


                            }
                            if (al == "GOWERandLEGENDRE")
                            {

                                try
                                {
                                    if ((a + 0.5 * (b + c) + d) == 0)
                                        GOWERandLEGENDREfolumla = 0.0f;
                                    else
                                        GOWERandLEGENDREfolumla = (a + d) / (a + 0.5 * (b + c) + d);

                                }
                                catch (System.Exception ex)
                                {
                                    GOWERandLEGENDREfolumla = 0.0f;
                                }

                                finally
                                {
                                    GOWERandLEGENDRE += GOWERandLEGENDREfolumla;
                                }


                            }
                            if (al == "INTERSECTION")
                            {

                                try
                                {
                                    INTERSECTIONfolumla = a;

                                }
                                catch (System.Exception ex)
                                {
                                    INTERSECTIONfolumla = 0.0f;
                                }

                                finally
                                {
                                    INTERSECTION += INTERSECTIONfolumla;
                                }


                            }
                            if (al == "INNERPRODUCT")
                            {

                                try
                                {
                                    INNERPRODUCTfolumla = a + d;

                                }
                                catch (System.Exception ex)
                                {
                                    INNERPRODUCTfolumla = 0.0f;
                                }

                                finally
                                {
                                    INNERPRODUCT += INNERPRODUCTfolumla;
                                }


                            }
                            if (al == "RUSSELLandRAO")
                            {

                                try
                                {
                                    if ((a + b + c + d) == 0)
                                        RUSSELLandRAOfolumla = 0.0f;
                                    else
                                        RUSSELLandRAOfolumla = a / (a + b + c + d);

                                }
                                catch (System.Exception ex)
                                {
                                    RUSSELLandRAOfolumla = 0.0f;
                                }

                                finally
                                {
                                    RUSSELLandRAO += RUSSELLandRAOfolumla;
                                }


                            }
                            if (al == "HAMMING")
                            {

                                try
                                {
                                    HAMMINGfolumla = b + c;
                                    // HAMMINGfolumla = 1/HAMMINGfolumla;

                                }
                                catch (System.Exception ex)
                                {
                                    HAMMINGfolumla = 0.0f;
                                }

                                finally
                                {
                                    HAMMING += HAMMINGfolumla;
                                }


                            }
                            if (al == "EUCLID")
                            {

                                try
                                {
                                    EUCLIDfolumla = Sqrt(b + c);
                                    // EUCLIDfolumla = 1/EUCLIDfolumla;

                                }
                                catch (System.Exception ex)
                                {
                                    EUCLIDfolumla = 0.0f;
                                }

                                finally
                                {
                                    EUCLID += EUCLIDfolumla;
                                }


                            }
                            if (al == "SQUARED_EUCLID")
                            {

                                try
                                {
                                    SQUARED_EUCLIDfolumla = Sqrt(Math.Pow((b + c), 2));
                                    //SQUARED_EUCLIDfolumla = 1/SQUARED_EUCLIDfolumla;
                                }
                                catch (System.Exception ex)
                                {
                                    SQUARED_EUCLIDfolumla = 0.0f;
                                }

                                finally
                                {
                                    SQUARED_EUCLID += SQUARED_EUCLIDfolumla;
                                }


                            }
                            if (al == "CANBERRA")
                            {

                                try
                                {
                                    CANBERRAfolumla = Math.Pow((b + c), 1);
                                    //CANBERRAfolumla = 1/CANBERRAfolumla;

                                }
                                catch (System.Exception ex)
                                {
                                    CANBERRAfolumla = 0.0f;
                                }

                                finally
                                {
                                    CANBERRA += CANBERRAfolumla;
                                }


                            }
                            if (al == "MANHATTAN")
                            {

                                try
                                {
                                    MANHATTANfolumla = b + c;
                                    // MANHATTANfolumla = 1/MANHATTANfolumla;

                                }
                                catch (System.Exception ex)
                                {
                                    MANHATTANfolumla = 0.0f;
                                }

                                finally
                                {
                                    MANHATTAN += MANHATTANfolumla;
                                }


                            }
                            if (al == "MEAN_MANHATTAN")
                            {

                                try
                                {
                                    if ((a + b + c + d) == 0)
                                        MEAN_MANHATTANfolumla = 0.0f;
                                    else
                                        MEAN_MANHATTANfolumla = (b + c) / (a + b + c + d);
                                    // MEAN_MANHATTANfolumla = 1/MEAN_MANHATTANfolumla;

                                }
                                catch (System.Exception ex)
                                {
                                    MEAN_MANHATTANfolumla = 0.0f;
                                }

                                finally
                                {
                                    MEAN_MANHATTAN += MEAN_MANHATTANfolumla;
                                }


                            }
                            if (al == "CITYBLOCK")
                            {

                                try
                                {
                                    CITYBLOCKfolumla = b + c;
                                    // CITYBLOCKfolumla = 1/CITYBLOCKfolumla;

                                }
                                catch (System.Exception ex)
                                {
                                    CITYBLOCKfolumla = 0.0f;
                                }

                                finally
                                {
                                    CITYBLOCK += CITYBLOCKfolumla;
                                }


                            }
                            if (al == "MINKOWSK")
                            {

                                try
                                {
                                    MINKOWSKfolumla = Math.Pow((b + c), (1));
                                    //  MINKOWSKfolumla = 1/MINKOWSKfolumla;

                                }
                                catch (System.Exception ex)
                                {
                                    MINKOWSKfolumla = 0.0f;
                                }

                                finally
                                {
                                    MINKOWSK += MINKOWSKfolumla;
                                }


                            }
                            if (al == "VARI")
                            {

                                try
                                {
                                    if ((4 * (a + b + c + d)) == 0)
                                        VARIfolumla = 0.0f;
                                    else
                                        VARIfolumla = (b + c) / (4 * (a + b + c + d));
                                    //  VARIfolumla = 1/VARIfolumla;

                                }
                                catch (System.Exception ex)
                                {
                                    VARIfolumla = 0.0f;
                                }

                                finally
                                {
                                    VARI += VARIfolumla;
                                }


                            }
                            if (al == "SIZEDIFFERENCE")
                            {

                                try
                                {
                                    if (Math.Pow((a + b + c + d), 2) == 0)
                                        SIZEDIFFERENCEfolumla = 0.0f;
                                    else
                                        SIZEDIFFERENCEfolumla = (Math.Pow((b + c), 2)) / (Math.Pow((a + b + c + d), 2));
                                    // SIZEDIFFERENCEfolumla = 1/SIZEDIFFERENCEfolumla;
                                }
                                catch (System.Exception ex)
                                {
                                    SIZEDIFFERENCEfolumla = 0.0f;
                                }

                                finally
                                {
                                    SIZEDIFFERENCE += SIZEDIFFERENCEfolumla;
                                }


                            }
                            if (al == "SHAPEDIFFERENCE")
                            {

                                try
                                {
                                    if (Math.Pow((a + b + c + d), 2) == 0)
                                        SHAPEDIFFERENCEfolumla = 0.0f;
                                    else
                                        SHAPEDIFFERENCEfolumla = ((a + b + c + d) * (b + c) - Math.Pow((b - c), 2)) / (Math.Pow((a + b + c + d), 2));
                                    //  SHAPEDIFFERENCEfolumla = 1 / SHAPEDIFFERENCEfolumla;
                                }
                                catch (System.Exception ex)
                                {
                                    SHAPEDIFFERENCEfolumla = 0.0f;
                                }

                                finally
                                {
                                    SHAPEDIFFERENCE += SHAPEDIFFERENCEfolumla;
                                }


                            }
                            if (al == "PATTERNDIFFERENCE")
                            {

                                try
                                {
                                    if (Math.Pow((a + b + c + d), 2) == 0)
                                        PATTERNDIFFERENCEfolumla = 0.0f;
                                    else
                                        PATTERNDIFFERENCEfolumla = (4 * b * c) / (Math.Pow((a + b + c + d), 2));
                                    //  PATTERNDIFFERENCEfolumla = 1 / PATTERNDIFFERENCEfolumla;
                                }
                                catch (System.Exception ex)
                                {
                                    PATTERNDIFFERENCEfolumla = 0.0f;
                                }

                                finally
                                {
                                    PATTERNDIFFERENCE += PATTERNDIFFERENCEfolumla;
                                }


                            }
                            if (al == "LANCEandWILLIAMS")
                            {

                                try
                                {
                                    if ((2 * a + b + c) == 0)
                                        LANCEandWILLIAMSfolumla = 0.0f;
                                    else
                                        LANCEandWILLIAMSfolumla = (b + c) / (2 * a + b + c);
                                    //  LANCEandWILLIAMSfolumla = 1 / LANCEandWILLIAMSfolumla;

                                }
                                catch (System.Exception ex)
                                {
                                    LANCEandWILLIAMSfolumla = 0.0f;
                                }

                                finally
                                {
                                    LANCEandWILLIAMS += LANCEandWILLIAMSfolumla;
                                }


                            }
                            if (al == "BRAYandCURTIS")
                            {

                                try
                                {
                                    if ((2 * a + b + c) == 0)
                                        BRAYandCURTISfolumla = 0.0f;
                                    else
                                        BRAYandCURTISfolumla = (b + c) / (2 * a + b + c);
                                    //  BRAYandCURTISfolumla = 1 / BRAYandCURTISfolumla;

                                }
                                catch (System.Exception ex)
                                {
                                    BRAYandCURTISfolumla = 0.0f;
                                }

                                finally
                                {
                                    BRAYandCURTIS += BRAYandCURTISfolumla;
                                }


                            }
                            if (al == "HELLINGER")
                            {

                                try
                                {
                                    if (Sqrt((a + b) * (a + c)) == 0)
                                        HELLINGERfolumla = 2 * Sqrt(1 - 0);
                                    else
                                        HELLINGERfolumla = 2 * Sqrt(1 - (a / Sqrt((a + b) * (a + c))));
                                    //  HELLINGERfolumla = 1 / HELLINGERfolumla;
                                }
                                catch (System.Exception ex)
                                {
                                    HELLINGERfolumla = 0.0f;
                                }

                                finally
                                {
                                    HELLINGER += HELLINGERfolumla;
                                }


                            }
                            if (al == "CHORD")
                            {

                                try
                                {
                                    if (Sqrt((a + b) * (a + c)) == 0)
                                        CHORDfolumla = Sqrt(2 * (1 - 0));
                                    else
                                        CHORDfolumla = Sqrt(2 * (1 - (a / Sqrt((a + b) * (a + c)))));
                                    // CHORDfolumla = 1 / CHORDfolumla;
                                }
                                catch (System.Exception ex)
                                {
                                    CHORDfolumla = 0.0f;
                                }

                                finally
                                {
                                    CHORD += CHORDfolumla;
                                }


                            }
                            if (al == "COSINE")
                            {

                                try
                                {
                                    // Logger.WriteLine((Math.Pow(Sqrt((a + b) * (a + c)), 2)).ToString());

                                    if ((Math.Pow(Sqrt((a + b) * (a + c)), 2)) == 0)
                                        COSINEfolumla = 0.0f;
                                    else
                                        COSINEfolumla = a / (Math.Pow(Sqrt((a + b) * (a + c)), 2));
                                }
                                catch (System.Exception ex)
                                {
                                    COSINEfolumla = 0.0f;
                                }

                                finally
                                {
                                    COSINE += COSINEfolumla;
                                }


                            }
                            if (al == "GILBERTandWELLS")
                            {

                                try
                                {
                                    if (a == 0)
                                    {
                                        GILBERTandWELLSfolumla = 0 - Math.Log((a + b + c + d)) - Math.Log((a + b) / (a + b + c + d)) - Math.Log((a + c) / (a + b + c + d));
                                        //Logger.WriteLine("a");
                                        //Logger.WriteLine(Math.Log(a).ToString());
                                        //Logger.WriteLine(Math.Log((a + b + c + d)).ToString());
                                        //Logger.WriteLine((Math.Log(a) - Math.Log((a + b + c + d)) - Math.Log((a + b) / (a + b + c + d)) - Math.Log((a + c) / (a + b + c + d))).ToString());
                                    }
                                    else if ((a + b) == 0)
                                    {
                                        GILBERTandWELLSfolumla = 0;
                                    }
                                    else if ((a + c) == 0)
                                    {
                                        GILBERTandWELLSfolumla = 0;
                                    }
                                    else {
                                        GILBERTandWELLSfolumla = Math.Log(a) - Math.Log((a + b + c + d)) - Math.Log((a + b) / (a + b + c + d)) - Math.Log((a + c) / (a + b + c + d));
                                        //    Logger.WriteLine("b");
                                        //    Logger.WriteLine(Math.Log(a).ToString());
                                        //    Logger.WriteLine(Math.Log((a + b + c + d)).ToString());
                                        //    Logger.WriteLine((Math.Log(a) - Math.Log((a + b + c + d)) - Math.Log((a + b) / (a + b + c + d)) - Math.Log((a + c) / (a + b + c + d))).ToString());
                                    }
                                }
                                catch (System.Exception ex)
                                {
                                    GILBERTandWELLSfolumla = 0.0f;
                                }

                                finally
                                {
                                    GILBERTandWELLS += GILBERTandWELLSfolumla;
                                }


                            }
                            if (al == "OCHIAI1")
                            {

                                try
                                {
                                    if (Sqrt((a + b) * (a + c)) == 0)
                                        OCHIAI1folumla = 0.0f;
                                    else
                                        OCHIAI1folumla = a / (Sqrt((a + b) * (a + c)));

                                }
                                catch (System.Exception ex)
                                {
                                    OCHIAI1folumla = 0.0f;
                                }

                                finally
                                {
                                    OCHIAI1 += OCHIAI1folumla;
                                }


                            }
                            if (al == "FORBESI")
                            {

                                try
                                {
                                    if (((a + b) * (a + c)) == 0)
                                        FORBESIfolumla = 0.0f;
                                    else
                                        FORBESIfolumla = (a + b + c + d) * a / ((a + b) * (a + c));
                                }
                                catch (System.Exception ex)
                                {
                                    FORBESIfolumla = 0.0f;
                                }

                                finally
                                {
                                    FORBESI += FORBESIfolumla;
                                }


                            }
                            if (al == "FOSSUM")
                            {

                                try
                                {
                                    if (((a + b) * (a + c)) == 0)
                                        FOSSUMfolumla = 0.0f;
                                    else
                                        FOSSUMfolumla = ((a + b + c + d) * (Math.Pow((a - 0.5), 2))) / ((a + b) * (a + c));
                                    //Logger.WriteLine("FOSSUMfolumla:" + FOSSUMfolumla);
                                }
                                catch (System.Exception ex)
                                {
                                    FOSSUMfolumla = 0.0f;
                                }

                                finally
                                {
                                    FOSSUM += FOSSUMfolumla;
                                }


                            }
                            if (al == "SORGENFREI")
                            {

                                try
                                {
                                    if (((a + b) * (a + c)) == 0)
                                        SORGENFREIfolumla = 0.0f;
                                    else
                                        SORGENFREIfolumla = (Math.Pow(a, 2)) / ((a + b) * (a + c));
                                }
                                catch (System.Exception ex)
                                {
                                    SORGENFREIfolumla = 0.0f;
                                }

                                finally
                                {
                                    SORGENFREI += SORGENFREIfolumla;
                                }


                            }
                            if (al == "MOUNTFORD")
                            {

                                try
                                {
                                    if (0.5 * ((a * b) + (a * c)) + b * c == 0)
                                        MOUNTFORDfolumla = 0.0f;
                                    else
                                        MOUNTFORDfolumla = a / (0.5 * ((a * b) + (a * c)) + b * c);
                                }
                                catch (System.Exception ex)
                                {
                                    MOUNTFORDfolumla = 0.0f;
                                }

                                finally
                                {
                                    MOUNTFORD += MOUNTFORDfolumla;
                                }


                            }
                            if (al == "OTSUKA")
                            {

                                try
                                {
                                    if (Math.Pow(((a + b) * (a + c)), 0.5) == 0)
                                        OTSUKAfolumla = 0.0f;
                                    else
                                        OTSUKAfolumla = a / (Math.Pow(((a + b) * (a + c)), 0.5));
                                }
                                catch (System.Exception ex)
                                {
                                    OTSUKAfolumla = 0.0f;
                                }

                                finally
                                {
                                    OTSUKA += OTSUKAfolumla;
                                }


                            }
                            if (al == "MCCONNAUGHEY")
                            {

                                try
                                {
                                    if ((a + b) * (a + c) == 0)
                                        MCCONNAUGHEYfolumla = 0.0f;
                                    else
                                        MCCONNAUGHEYfolumla = (Math.Pow(a, 2) - (b * c)) / ((a + b) * (a + c));
                                }
                                catch (System.Exception ex)
                                {
                                    MCCONNAUGHEYfolumla = 0.0f;
                                }

                                finally
                                {
                                    MCCONNAUGHEY += MCCONNAUGHEYfolumla;
                                }


                            }
                            if (al == "TARWID")
                            {

                                try
                                {
                                    if (((a + b + c + d) * a) + (a + b) * (a + c) == 0)
                                        TARWIDfolumla = 0.0f;
                                    else
                                        TARWIDfolumla = (((a + b + c + d) * a) - (a + b) * (a + c)) / ((a + b + c + d) * a + (a + b) * (a + c));
                                }
                                catch (System.Exception ex)
                                {
                                    TARWIDfolumla = 0.0f;
                                }

                                finally
                                {
                                    TARWID += TARWIDfolumla;
                                }


                            }
                            if (al == "KULCZYNSK2")
                            {

                                try
                                {
                                    if ((a + b) * (a + c) == 0)
                                        KULCZYNSK2folumla = 0.0f;
                                    else
                                        KULCZYNSK2folumla = ((a / 2) * (2 * a + b + c)) / ((a + b) * (a + c));

                                }
                                catch (System.Exception ex)
                                {
                                    KULCZYNSK2folumla = 0.0f;
                                }

                                finally
                                {
                                    KULCZYNSK2 += KULCZYNSK2folumla;
                                }


                            }
                            if (al == "DRIVERandKROEBER")
                            {

                                try
                                {
                                    if ((a + b) == 0)
                                        Ldenominator = 0.0f;
                                    else
                                        Ldenominator = 1 / (a + b);
                                    if ((a + c) == 0)
                                        Rdenominator = 0.0f;
                                    else
                                        Rdenominator = 1 / (a + c);

                                    if ((Ldenominator + Rdenominator) == 0)
                                        DRIVERandKROEBERfolumla = 0.0f;
                                    else
                                        DRIVERandKROEBERfolumla = (a / 2) * (Ldenominator + Rdenominator);

                                }
                                catch (System.Exception ex)
                                {
                                    DRIVERandKROEBERfolumla = 0.0f;
                                }

                                finally
                                {
                                    DRIVERandKROEBER += DRIVERandKROEBERfolumla;
                                }


                            }
                            if (al == "JOHNSON")
                            {

                                try
                                {
                                    if ((a + b) == 0)
                                        Ldenominator = 0.0f;
                                    else
                                        Ldenominator = a / (a + b);
                                    if ((a + c) == 0)
                                        Rdenominator = 0.0f;
                                    else
                                        Rdenominator = a / (a + c);

                                    if ((Ldenominator + Rdenominator) == 0)
                                        JOHNSONfolumla = 0.0f;
                                    else
                                        JOHNSONfolumla = (Ldenominator + Rdenominator);

                                }
                                catch (System.Exception ex)
                                {
                                    JOHNSONfolumla = 0.0f;
                                }

                                finally
                                {
                                    JOHNSON += JOHNSONfolumla;
                                }


                            }
                            if (al == "DENNIS")
                            {

                                try
                                {
                                    if (((a + b + c + d) * (a + b) * (a + c)) == 0)
                                        DENNISfolumla = 0.0f;
                                    else
                                        DENNISfolumla = (a * d - b * c) / (Sqrt((a + b + c + d) * (a + b) * (a + c)));

                                }
                                catch (System.Exception ex)
                                {
                                    DENNISfolumla = 0.0f;
                                }

                                finally
                                {
                                    DENNIS += DENNISfolumla;
                                }


                            }
                            if (al == "SIMPSON")
                            {

                                try
                                {
                                    if (Math.Min((a + b), (a + c)) == 0)
                                        SIMPSONfolumla = 0.0f;
                                    else
                                        SIMPSONfolumla = a / Math.Min((a + b), (a + c));

                                }
                                catch (System.Exception ex)
                                {
                                    SIMPSONfolumla = 0.0f;
                                }

                                finally
                                {
                                    SIMPSON += SIMPSONfolumla;
                                }


                            }
                            if (al == "BRAUNandBANQUET")
                            {

                                try
                                {
                                    if (Math.Max((a + b), (a + c)) == 0)
                                        BRAUNandBANQUETfolumla = 0.0f;
                                    else
                                        BRAUNandBANQUETfolumla = a / Math.Max((a + b), (a + c));

                                }
                                catch (System.Exception ex)
                                {
                                    BRAUNandBANQUETfolumla = 0.0f;
                                }

                                finally
                                {
                                    BRAUNandBANQUET += BRAUNandBANQUETfolumla;
                                }


                            }
                            if (al == "FAGERandMCGOWAN")
                            {

                                try
                                {

                                    if (Sqrt((a + b) * (a + c)) == 0)
                                        FAGERandMCGOWANfolumla = 0 - (Math.Max((a + b), (a + c)) / 2);
                                    else
                                        FAGERandMCGOWANfolumla = (a / Sqrt((a + b) * (a + c))) - (Math.Max((a + b), (a + c)) / 2);

                                }
                                catch (System.Exception ex)
                                {
                                    FAGERandMCGOWANfolumla = 0.0f;
                                }

                                finally
                                {
                                    FAGERandMCGOWAN += FAGERandMCGOWANfolumla;
                                }


                            }
                            if (al == "FORBES2")
                            {

                                try
                                {
                                    if ((((a + b + c + d) * Math.Min((a + b), (a + c))) - (a + b) * (a + c)) == 0)
                                        FORBES2folumla = 0.0f;
                                    else
                                        FORBES2folumla = (((a + b + c + d) * a) - ((a + b) * (a + c))) / (((a + b + c + d) * Math.Min((a + b), (a + c))) - (a + b) * (a + c));
                                }
                                catch (System.Exception ex)
                                {
                                    FORBES2folumla = 0.0f;
                                }

                                finally
                                {
                                    FORBES2 += FORBES2folumla;
                                }


                            }
                            if (al == "SOKALandSNEATH4")
                            {
                                double one = 0;
                                double two = 0;
                                double three = 0;

                                try
                                {

                                    if ((a + b) == 0)
                                    {
                                        one = 0;
                                    }
                                    else
                                    {
                                        one = (a / (a + b));
                                    }

                                    if ((a + c) == 0)
                                    {
                                        two = 0;
                                    }
                                    else
                                    {
                                        two = (a / (a + c));
                                    }

                                    if ((b + d) == 0)
                                    {
                                        three = 0;
                                    }
                                    else
                                    {
                                        three = (d / (b + d));
                                    }

                                    SOKALandSNEATH4folumla = (one + two + three + three) / 4;

                                }
                                catch (System.Exception ex)
                                {
                                    SOKALandSNEATH4folumla = 0.0f;
                                }

                                finally
                                {
                                    SOKALandSNEATH4 += SOKALandSNEATH4folumla;
                                }


                            }

                            if (al == "GOWER")
                            {
                                try
                                {
                                    if (((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                        GOWERfolumla = 0.0f;
                                    else
                                        GOWERfolumla = (a + d) / Sqrt((a + b) * (a + c) * (b + d) * (c + d));

                                }
                                catch (System.Exception ex)
                                {
                                    GOWERfolumla = 0.0f;
                                }
                                finally
                                {
                                    GOWER += GOWERfolumla;
                                }
                            }

                            if (al == "PEARSON1")
                            {
                                try
                                {
                                    if (((a + b) * (a + c) * (c + d) * (b + d)) == 0)
                                        PEARSON1folumla = 0.0f;
                                    else
                                        PEARSON1folumla = (((a + b + c + d) * (Math.Pow((a * d - b * c), 2))) / ((a + b) * (a + c) * (c + d) * (b + d)));
                                }
                                catch (System.Exception ex)
                                {
                                    PEARSON1folumla = 0.0f;
                                }
                                finally
                                {
                                    PEARSON1 += PEARSON1folumla;
                                }
                            }

                            if (al == "PEARSON2")
                            {
                                try
                                {
                                    if (((a + b) * (a + c) * (c + d) * (b + d)) == 0)
                                    {
                                        PEARSON2folumla = 0.0f;
                                    }
                                    else if ((a * d - b * c) == 0)
                                    {
                                        PEARSON2folumla = 0.0f;
                                    }
                                    else
                                    {
                                        PEARSON2folumla = Math.Pow((((a + b + c + d) * Math.Pow((a * d - b * c), 2)) / ((a + b) * (a + c) * (c + d) * (b + d)))
                                            / ((a + b + c + d) + (((a + b + c + d) * Math.Pow((a * d - b * c), 2)) / ((a + b) * (a + c) * (c + d) * (b + d)))), 0.5);





                                        //(((a + b + c + d) * Math.Pow((a * d - b * c), 2)) / ((a + b) * (a + c) * (c + d) * (b + d))) /
                                        //   ((a + b + c + d)+(((a + b + c + d) * Math.Pow((a * d - b * c), 2)) / ((a + b) * (a + c) * (c + d) * (b + d)));

                                    }


                                }
                                catch (System.Exception ex)
                                {
                                    PEARSON2folumla = 0.0f;
                                }
                                finally
                                {
                                    PEARSON2 += PEARSON2folumla;
                                }
                            }

                            if (al == "PEARSON3")
                            {
                                try
                                {
                                    if (((a + b) * (a + c) * (c + d) * (b + d)) == 0)
                                    {
                                        PEARSON3folumla = 0.0f;
                                    }
                                    else if (Sqrt(a + b) * (a + c) * (b + d) * (c + d) == 0)
                                    {
                                        PEARSON3folumla = 0.0f;
                                    }
                                    else if ((a * d - b * c) == 0)
                                    {
                                        PEARSON3folumla = 0.0f;
                                    }
                                    else
                                    {
                                        if (((((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)))) / ((a + b + c + d) + (((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)))))) < 0)
                                        {
                                            double temp = -((((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)))) / ((a + b + c + d) + (((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d))))));
                                            // Logger.WriteLine("temp:" + temp);

                                            PEARSON3folumla = Math.Pow(temp, 0.5);
                                            // Logger.WriteLine("PEARSON3folumla:" + PEARSON3folumla);
                                            PEARSON3folumla = -PEARSON3folumla;
                                            // Logger.WriteLine("PEARSON3folumla2:" + PEARSON3folumla);
                                        }
                                        else
                                        {
                                            PEARSON3folumla = Math.Pow((((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)))) / ((a + b + c + d) + (((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d))))), 0.5);
                                        }

                                    }

                                    //Logger.WriteLine("1:"+(a * d - b * c));
                                    //Logger.WriteLine("2:" + (Sqrt((a + b) * (a + c) * (b + d) * (c + d))).ToString());
                                    //Logger.WriteLine("3:" + (a + b + c + d));
                                    //Logger.WriteLine("4:" + ((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d))));
                                    //Logger.WriteLine("5:" + (((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)))) / ((a + b + c + d) + (((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d))))));
                                    //Logger.WriteLine("6:" + (Math.Pow((((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)))) / ((a + b + c + d) + (((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d))))), 0.5)).ToString());
                                    //Logger.WriteLine("=======================");

                                }
                                catch (System.Exception ex)
                                {
                                    PEARSON3folumla = 0.0f;
                                }
                                finally
                                {
                                    PEARSON3 += PEARSON3folumla;
                                }
                            }

                            if (al == "PEARSONandHERON1")
                            {
                                try
                                {
                                    if (((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                        PEARSONandHERON1folumla = 0.0f;
                                    else
                                        PEARSONandHERON1folumla = ((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)));
                                }
                                catch (System.Exception ex)
                                {
                                    PEARSONandHERON1folumla = 0.0f;
                                }
                                finally
                                {
                                    PEARSONandHERON1 += PEARSONandHERON1folumla;
                                }
                            }

                            if (al == "PEARSONandHERON2")
                            {
                                try
                                {
                                    if ((a * d) == 0)
                                    {
                                        PEARSONandHERON2folumla = 0.0f;

                                    }
                                    else if ((b * c) == 0)
                                    {
                                        PEARSONandHERON2folumla = 0.0f;
                                    }
                                    else
                                    {
                                        PEARSONandHERON2folumla = Math.Cos((Math.PI * Sqrt(b * c)) / (Sqrt(a * d) + Sqrt(b * c)));
                                    }

                                }
                                catch (System.Exception ex)
                                {
                                    PEARSONandHERON2folumla = 0.0f;
                                }
                                finally
                                {
                                    PEARSONandHERON2 += PEARSONandHERON2folumla;
                                }
                            }

                            if (al == "SOKALandSNEATH3")
                            {
                                try
                                {
                                    if ((b + c) == 0)
                                        SOKALandSNEATH3folumla = 0.0f;
                                    else
                                        SOKALandSNEATH3folumla = (a + d) / (b + c);
                                }
                                catch (System.Exception ex)
                                {
                                    SOKALandSNEATH3folumla = 0.0f;
                                }
                                finally
                                {
                                    SOKALandSNEATH3 += SOKALandSNEATH3folumla;
                                }
                            }

                            if (al == "SOKALandSNEATH5")
                            {
                                try
                                {
                                    if (((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                        SOKALandSNEATH5folumla = 0.0f;
                                    else
                                        SOKALandSNEATH5folumla = a * d / ((a + b) * (a + c) * (b + d) * Math.Pow((c + d), 0.5));
                                }
                                catch (System.Exception ex)
                                {
                                    SOKALandSNEATH5folumla = 0.0f;
                                }
                                finally
                                {
                                    SOKALandSNEATH5 += SOKALandSNEATH5folumla;
                                }
                            }
                            if (al == "COLE")
                            {
                                try
                                {
                                    if ((Math.Pow((a * d - b * c), 2) - ((a + b) * (a + c) * (b + d) * (c + d))) == 0)
                                    {
                                        COLEfolumla = 0.0f;
                                        //Logger.WriteLine("COLEfolumla1: " + COLEfolumla);
                                    }

                                    else
                                    {
                                        COLEfolumla = (Sqrt(2) * ((a * d - b * c))) / (Sqrt(Math.Pow((a * d - b * c), 2) - ((a + b) * (a + c) * (b + d) * (c + d))));
                                        //Logger.WriteLine("COLEfolumla2: " + COLEfolumla);

                                    }

                                }
                                catch (System.Exception ex)
                                {
                                    COLEfolumla = 0.0f;
                                }
                                finally
                                {
                                    COLE += COLEfolumla;
                                }
                            }


                            if (al == "STILES")
                            {
                                try
                                {
                                    if (((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                    {
                                        STILESfolumla = 0.0f;
                                    }

                                    else if (Math.Pow((Math.Abs(a * d - b * c) - ((a + b + c + d) / 2)), 2) / ((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                    {
                                        STILESfolumla = 0.0f;

                                    }
                                    else
                                    {

                                        STILESfolumla = Math.Log10((((a + b + c + d) * Math.Pow((Math.Abs(a * d - b * c) - ((a + b + c + d) / 2)), 2)) / ((a + b) * (a + c) * (b + d) * (c + d))));

                                        //Logger.WriteLine("1:" + Math.Log10(a + b + c + d));
                                        //Logger.WriteLine("2:" + Math.Pow((Math.Abs(a * d - b * c) - ((a + b + c + d) / 2)), 2));
                                        //Logger.WriteLine("3:" + ((a + b) * (a + c) * (b + d) * (c + d)));
                                        //Logger.WriteLine("4:" + Math.Log10(((a + b + c + d) * Math.Pow((Math.Abs(a * d - b * c) - ((a + b + c + d) / 2)), 2)) / ((a + b) * (a + c) * (b + d) * (c + d))));
                                        //Logger.WriteLine("5:" + Math.Pow((Math.Abs(a * d - b * c) - ((a + b + c + d) / 2)), 2) / ((a + b) * (a + c) * (b + d) * (c + d)));
                                        //Logger.WriteLine("STILESfolumla:" + STILESfolumla.ToString());
                                        //Logger.WriteLine("=============");
                                    }


                                }
                                catch (System.Exception ex)
                                {
                                    STILESfolumla = 0.0f;
                                }
                                finally
                                {
                                    STILES += STILESfolumla;
                                }
                            }

                            if (al == "OCHIAI2")
                            {
                                try
                                {
                                    if ((Sqrt((a + b) * (a + c) * (b + d) * (c + d))) == 0)
                                        OCHIAI2folumla = 0.0f;
                                    else
                                        OCHIAI2folumla = (a * d) / (Sqrt((a + b) * (a + c) * (b + d) * (c + d)));
                                }
                                catch (System.Exception ex)
                                {
                                    OCHIAI2folumla = 0.0f;
                                }
                                finally
                                {
                                    OCHIAI2 += OCHIAI2folumla;
                                }
                            }

                            if (al == "YULEQ")
                            {
                                try
                                {
                                    if ((a * d + b * c) == 0)
                                        YULEQfolumla = 0.0f;
                                    else
                                        YULEQfolumla = (a * d - b * c) / (a * d + b * c);
                                }
                                catch (System.Exception ex)
                                {
                                    YULEQfolumla = 0.0f;
                                }
                                finally
                                {
                                    YULEQ += YULEQfolumla;
                                }
                            }

                            if (al == "D_YULEQ")
                            {
                                try
                                {
                                    if ((a * d + b * c) == 0)
                                        D_YULEQfolumla = 0.0f;
                                    else
                                        D_YULEQfolumla = 2 * b * c / (a * d + b * c);
                                    //  D_YULEQfolumla = 1/D_YULEQfolumla;
                                }
                                catch (System.Exception ex)
                                {
                                    D_YULEQfolumla = 0.0f;
                                }
                                finally
                                {
                                    D_YULEQ += D_YULEQfolumla;
                                }
                            }

                            if (al == "YULEw")
                            {
                                try
                                {
                                    if ((Sqrt(a * d) + Sqrt(b * c)) == 0)
                                        YULEwfolumla = 0.0f;
                                    else
                                        YULEwfolumla = (Sqrt(a * d) - Sqrt(b * c)) / (Sqrt(a * d) + Sqrt(b * c));
                                }
                                catch (System.Exception ex)
                                {
                                    YULEwfolumla = 0.0f;
                                }
                                finally
                                {
                                    YULEw += YULEwfolumla;
                                }
                            }

                            if (al == "KULCZYNSKI1")
                            {
                                try
                                {
                                    if ((b + c) == 0)
                                        KULCZYNSKI1folumla = 0.0f;
                                    else
                                        KULCZYNSKI1folumla = a / (b + c);
                                }
                                catch (System.Exception ex)
                                {
                                    KULCZYNSKI1folumla = 0.0f;
                                }
                                finally
                                {
                                    KULCZYNSKI1 += KULCZYNSKI1folumla;
                                }
                            }

                            if (al == "TANIMOTO")
                            {
                                try
                                {
                                    if (((a + b) + (a + c) - a) == 0)
                                        TANIMOTOfolumla = 0.0f;
                                    else
                                        TANIMOTOfolumla = a / ((a + b) + (a + c) - a);
                                }
                                catch (System.Exception ex)
                                {
                                    TANIMOTOfolumla = 0.0f;
                                }
                                finally
                                {
                                    TANIMOTO += TANIMOTOfolumla;
                                }
                            }

                            if (al == "DISPERSON")
                            {
                                try
                                {
                                    if ((Math.Pow((a + b + c + d), 2)) == 0)
                                        DISPERSONfolumla = 0.0f;
                                    else
                                        DISPERSONfolumla = (a * d - b * c) / (Math.Pow((a + b + c + d), 2));
                                }
                                catch (System.Exception ex)
                                {
                                    DISPERSONfolumla = 0.0f;
                                }
                                finally
                                {
                                    DISPERSON += DISPERSONfolumla;
                                }
                            }

                            if (al == "HAMANN")
                            {
                                try
                                {
                                    if ((a + b + c + d) == 0)
                                        HAMANNfolumla = 0.0f;
                                    else
                                        HAMANNfolumla = ((a + d) - (b + c)) / (a + b + c + d);
                                }
                                catch (System.Exception ex)
                                {
                                    HAMANNfolumla = 0.0f;
                                }
                                finally
                                {
                                    HAMANN += HAMANNfolumla;
                                }
                            }

                            if (al == "MICHAEL")
                            {
                                try
                                {
                                    if ((Math.Pow((a + d), 2) + Math.Pow((b + c), 2)) == 0)
                                        MICHAELfolumla = 0.0f;
                                    else
                                        MICHAELfolumla = 4 * (a * d - b * c) / (Math.Pow((a + d), 2) + Math.Pow((b + c), 2));
                                }
                                catch (System.Exception ex)
                                {
                                    MICHAELfolumla = 0.0f;
                                }
                                finally
                                {
                                    MICHAEL += MICHAELfolumla;
                                }
                            }

                            if (al == "GOODMANandKRUSKAL")
                            {
                                try
                                {
                                    if ((2 * (a + b + c + d) - (Math.Max(a + c, b + d) + Math.Max(a + b, c + d))) == 0)
                                        GOODMANandKRUSKALfolumla = 0.0f;
                                    else
                                        GOODMANandKRUSKALfolumla = ((Math.Max(a, b) + Math.Max(c, d) + Math.Max(a, c) + Math.Max(b, d)) - (Math.Max(a + c, b + d) + Math.Max(a + b, c + d))) / (2 * (a + b + c + d) - (Math.Max(a + c, b + d) + Math.Max(a + b, c + d)));
                                }
                                catch (System.Exception ex)
                                {
                                    GOODMANandKRUSKALfolumla = 0.0f;
                                }
                                finally
                                {
                                    GOODMANandKRUSKAL += GOODMANandKRUSKALfolumla;
                                }
                            }

                            if (al == "ANDERBERG")
                            {
                                try
                                {
                                    if ((2 * (a + b + c + d)) == 0)
                                        ANDERBERGfolumla = 0.0f;
                                    else
                                        ANDERBERGfolumla = ((Math.Max(a, b) + Math.Max(c, d) + Math.Max(a, c) + Math.Max(b, d)) - (Math.Max(a + c, b + d) + Math.Max(a + b, c + d))) / (2 * (a + b + c + d));
                                }
                                catch (System.Exception ex)
                                {
                                    ANDERBERGfolumla = 0.0f;
                                }
                                finally
                                {
                                    ANDERBERG += ANDERBERGfolumla;
                                }
                            }

                            if (al == "BARONI_URBANIandBUSER1")
                            {
                                try
                                {
                                    if ((Sqrt(a * d) + a + b + c) == 0)
                                        BARONI_URBANIandBUSER1folumla = 0.0f;
                                    else
                                        BARONI_URBANIandBUSER1folumla = (Sqrt(a * d) + a) / (Sqrt(a * d) + a + b + c);
                                }
                                catch (System.Exception ex)
                                {
                                    BARONI_URBANIandBUSER1folumla = 0.0f;
                                }
                                finally
                                {
                                    BARONI_URBANIandBUSER1 += BARONI_URBANIandBUSER1folumla;
                                }
                            }
                            if (al == "BARONI_URBANIandBUSER2")
                            {
                                try
                                {
                                    if ((Sqrt(a * d) + a + b + c) == 0)
                                        BARONI_URBANIandBUSER2folumla = 0.0f;
                                    else
                                        BARONI_URBANIandBUSER2folumla = (Sqrt(a * d) + a - (b + c)) / (Sqrt(a * d) + a + b + c);
                                }
                                catch (System.Exception ex)
                                {
                                    BARONI_URBANIandBUSER2folumla = 0.0f;
                                }
                                finally
                                {
                                    BARONI_URBANIandBUSER2 += BARONI_URBANIandBUSER2folumla;
                                }
                            }
                            if (al == "PEIRCE")
                            {
                                try
                                {
                                    if (((a * b) + (2 * b * c) + (c * d)) == 0)
                                        PEIRCEfolumla = 0.0f;
                                    else
                                        PEIRCEfolumla = (a * b + b * c) / ((a * b) + (2 * b * c) + (c * d));
                                }
                                catch (System.Exception ex)
                                {
                                    PEIRCEfolumla = 0.0f;
                                }
                                finally
                                {
                                    PEIRCE += PEIRCEfolumla;
                                }
                            }
                            if (al == "EYRAUD")
                            {
                                try
                                {
                                    if (((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                        EYRAUDfolumla = 0.0f;
                                    else
                                        EYRAUDfolumla = (Math.Pow((a + b + c + d), 2) * ((a + b + c + d) * a - (a + b) * (a + c))) / ((a + b) * (a + c) * (b + d) * (c + d));
                                }
                                catch (System.Exception ex)
                                {
                                    EYRAUDfolumla = 0.0f;
                                }
                                finally
                                {
                                    EYRAUD += EYRAUDfolumla;
                                }

                            }





                        }
                        //실험 대상 기법2

                        dsP.Tables["PSourceCode"].Rows[row]["TARANTULA"] = Math.Round(TARANTULA, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["AMPLE"] = Math.Round(AMPLE, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["Jaccard"] = Math.Round(Jaccard, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["Dice"] = Math.Round(Dice, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["CZEKANOWSKI"] = Math.Round(CZEKANOWSKI, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["_3WJACCARD"] = Math.Round(_3WJACCARD, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["NEIandLI"] = Math.Round(NEIandLI, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SOKALandSNEATH_1"] = Math.Round(SOKALandSNEATH_1, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SOKALandMICHENER"] = Math.Round(SOKALandMICHENER, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SOKALandSNEATH2"] = Math.Round(SOKALandSNEATH2, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["ROGERandTANIMOTO"] = Math.Round(ROGERandTANIMOTO, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["FAITH"] = Math.Round(FAITH, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["GOWERandLEGENDRE"] = Math.Round(GOWERandLEGENDRE, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["INTERSECTION"] = Math.Round(INTERSECTION, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["INNERPRODUCT"] = Math.Round(INNERPRODUCT, 5);



                        dsP.Tables["PSourceCode"].Rows[row]["RUSSELLandRAO"] = Math.Round(RUSSELLandRAO, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["HAMMING"] = Math.Round(-HAMMING, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["EUCLID"] = Math.Round(-EUCLID, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SQUARED_EUCLID"] = Math.Round(-SQUARED_EUCLID, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["CANBERRA"] = Math.Round(-CANBERRA, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["MANHATTAN"] = Math.Round(-MANHATTAN, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["MEAN_MANHATTAN"] = Math.Round(-MEAN_MANHATTAN, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["CITYBLOCK"] = Math.Round(-CITYBLOCK, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["MINKOWSK"] = Math.Round(-MINKOWSK, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["VARI"] = Math.Round(-VARI, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SIZEDIFFERENCE"] = Math.Round(-SIZEDIFFERENCE, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SHAPEDIFFERENCE"] = Math.Round(-SHAPEDIFFERENCE, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["PATTERNDIFFERENCE"] = Math.Round(-PATTERNDIFFERENCE, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["LANCEandWILLIAMS"] = Math.Round(-LANCEandWILLIAMS, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["BRAYandCURTIS"] = Math.Round(-BRAYandCURTIS, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["HELLINGER"] = Math.Round(-HELLINGER, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["CHORD"] = Math.Round(-CHORD, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["COSINE"] = Math.Round(COSINE, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["GILBERTandWELLS"] = Math.Round(GILBERTandWELLS, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["OCHIAI1"] = Math.Round(OCHIAI1, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["FORBESI"] = Math.Round(FORBESI, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["FOSSUM"] = Math.Round(FOSSUM, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SORGENFREI"] = Math.Round(SORGENFREI, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["MOUNTFORD"] = Math.Round(MOUNTFORD, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["OTSUKA"] = Math.Round(OTSUKA, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["MCCONNAUGHEY"] = Math.Round(MCCONNAUGHEY, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["TARWID"] = Math.Round(TARWID, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["KULCZYNSK2"] = Math.Round(KULCZYNSK2, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["DRIVERandKROEBER"] = Math.Round(DRIVERandKROEBER, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["JOHNSON"] = Math.Round(JOHNSON, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["DENNIS"] = Math.Round(DENNIS, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SIMPSON"] = Math.Round(SIMPSON, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["BRAUNandBANQUET"] = Math.Round(BRAUNandBANQUET, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["FAGERandMCGOWAN"] = Math.Round(FAGERandMCGOWAN, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["FORBES2"] = Math.Round(FORBES2, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SOKALandSNEATH4"] = Math.Round(SOKALandSNEATH4, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["GOWER"] = Math.Round(GOWER, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["PEARSON1"] = Math.Round(PEARSON1, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["PEARSON2"] = Math.Round(PEARSON2, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["PEARSON3"] = Math.Round(PEARSON3, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["PEARSONandHERON1"] = Math.Round(PEARSONandHERON1, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["PEARSONandHERON2"] = Math.Round(PEARSONandHERON2, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SOKALandSNEATH3"] = Math.Round(SOKALandSNEATH3, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["SOKALandSNEATH5"] = Math.Round(SOKALandSNEATH5, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["COLE"] = Math.Round(COLE, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["STILES"] = Math.Round(STILES, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["OCHIAI2"] = Math.Round(OCHIAI2, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["YULEQ"] = Math.Round(YULEQ, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["D_YULEQ"] = Math.Round(-D_YULEQ, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["YULEw"] = Math.Round(YULEw, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["KULCZYNSKI1"] = Math.Round(KULCZYNSKI1, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["TANIMOTO"] = Math.Round(TANIMOTO, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["DISPERSON"] = Math.Round(DISPERSON, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["HAMANN"] = Math.Round(HAMANN, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["MICHAEL"] = Math.Round(MICHAEL, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["GOODMANandKRUSKAL"] = Math.Round(GOODMANandKRUSKAL, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["ANDERBERG"] = Math.Round(ANDERBERG, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["BARONI_URBANIandBUSER1"] = Math.Round(BARONI_URBANIandBUSER1, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["BARONI_URBANIandBUSER2"] = Math.Round(BARONI_URBANIandBUSER2, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["PEIRCE"] = Math.Round(PEIRCE, 5);
                        dsP.Tables["PSourceCode"].Rows[row]["EYRAUD"] = Math.Round(EYRAUD, 5);













                    }

                }

                TARANTULA = 0.0;
                AMPLE = 0.0;
                Jaccard = 0.0;
                Dice = 0.0;
                CZEKANOWSKI = 0.0;
                _3WJACCARD = 0.0;
                NEIandLI = 0.0;
                SOKALandSNEATH_1 = 0.0;
                SOKALandMICHENER = 0.0;
                SOKALandSNEATH2 = 0.0;
                ROGERandTANIMOTO = 0.0;
                FAITH = 0.0;
                GOWERandLEGENDRE = 0.0;
                INTERSECTION = 0.0;
                INNERPRODUCT = 0.0;
                RUSSELLandRAO = 0.0;
                HAMMING = 0.0;
                EUCLID = 0.0;
                SQUARED_EUCLID = 0.0;
                CANBERRA = 0.0;
                MANHATTAN = 0.0;
                MEAN_MANHATTAN = 0.0;
                CITYBLOCK = 0.0;
                MINKOWSK = 0.0;
                VARI = 0.0;
                SIZEDIFFERENCE = 0.0;
                SHAPEDIFFERENCE = 0.0;
                PATTERNDIFFERENCE = 0.0;
                LANCEandWILLIAMS = 0.0;
                BRAYandCURTIS = 0.0;
                HELLINGER = 0.0;
                CHORD = 0.0;
                COSINE = 0.0;
                GILBERTandWELLS = 0.0;
                OCHIAI1 = 0.0;
                FORBESI = 0.0;
                FOSSUM = 0.0;
                SORGENFREI = 0.0;
                MOUNTFORD = 0.0;
                OTSUKA = 0.0;
                MCCONNAUGHEY = 0.0;
                TARWID = 0.0;
                KULCZYNSK2 = 0.0;
                DRIVERandKROEBER = 0.0;
                JOHNSON = 0.0;
                DENNIS = 0.0;
                SIMPSON = 0.0;
                BRAUNandBANQUET = 0.0;
                FAGERandMCGOWAN = 0.0;
                FORBES2 = 0.0;
                SOKALandSNEATH4 = 0.0;
                GOWER = 0.0;
                PEARSON1 = 0.0;
                PEARSON2 = 0.0;
                PEARSON3 = 0.0;
                PEARSONandHERON1 = 0.0;
                PEARSONandHERON2 = 0.0;
                SOKALandSNEATH3 = 0.0;
                SOKALandSNEATH5 = 0.0;
                COLE = 0.0;
                STILES = 0.0;
                OCHIAI2 = 0.0;
                YULEQ = 0.0;
                D_YULEQ = 0.0;
                YULEw = 0.0;
                KULCZYNSKI1 = 0.0;
                TANIMOTO = 0.0;
                DISPERSON = 0.0;
                HAMANN = 0.0;
                MICHAEL = 0.0;
                GOODMANandKRUSKAL = 0.0;
                ANDERBERG = 0.0;
                BARONI_URBANIandBUSER1 = 0.0;
                BARONI_URBANIandBUSER2 = 0.0;
                PEIRCE = 0.0;
                EYRAUD = 0.0;
                d = b = c = a = 0;



                //실험 보류 대상 기법
                //Tarantula = 0.0;
                //AMPLE = 0.0;
                //Jaccard = 0.0;
                //SEM1 = 0.0;
                //SEM2 = 0.0;
                //SEM3 = 0.0;
                //Naish = 0.0;
                //Ochiai = 0.0;
                //Zoltar = 0.0;
                //Kulczynski2 = 0.0;
                //Anderberg = 0.0;
                //M2 = 0.0;
                //Dice = 0.0;
                //PS = 0.0;
                //Wong3 = 0.0;
                //GP08 = 0.0;
                //GP10 = 0.0;
                //GP11 = 0.0;
                //GP13 = 0.0;
                //GP20 = 0.0;
                //GP26 = 0.0;
                //SorensenDice = 0.0;
                //Wong1 = 0.0;
                //SimpleMatching = 0.0;
                //Sokal = 0.0;
                //RogersTanimoto = 0.0;
                //Goodman = 0.0;
                //Hammingetc = 0.0;
                //Euclid = 0.0;
                //M1 = 0.0;
                //Hamann = 0.0;
                //Wong2 = 0.0;
                //RussellandRao = 0.0;
                //Cohen = 0.0;
                //GeometricMean = 0.0;
                //HarmonicMean = 0.0;
                //ArithmeticMean = 0.0;
                //Rogot1 = 0.0;
            }
        }



        void ComputeSuspiciousValue2(DataSet ds)
        {


            int Pcol_TC_ID = ds.Tables["PSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Prow_last = ds.Tables["PSourceCode"].Rows.Count;
            int Pcol_last = ds.Tables["PSourceCode"].Columns.Count;

            double d, b, c, a, blank = 0;

            string result;
            string mark;

            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {

                ds.Tables["PSourceCode"].Columns.Add("d");
                ds.Tables["PSourceCode"].Columns.Add("b");
                ds.Tables["PSourceCode"].Columns.Add("c");
                ds.Tables["PSourceCode"].Columns.Add("a");
                ds.Tables["PSourceCode"].Columns.Add("unnecessary");





                foreach (string al in split_al)
                {

                    ds.Tables["PSourceCode"].Columns.Add(al, typeof(string));
                    ds.Tables["PSourceCode"].Columns.Add(al + "_Rank", typeof(string));
                    ds.Tables["PSourceCode"].Columns[al].ColumnName = al;
                    ds.Tables["PSourceCode"].Columns[al + "_Rank"].ColumnName = al + "_Rank";

                }

            }
            catch (System.Exception ex)
            {

            }
            finally
            {

                ds.Tables["PSourceCode"].Columns["d"].SetOrdinal(ds.Tables["PSourceCode"].Columns.Count - 1);
                ds.Tables["PSourceCode"].Columns["b"].SetOrdinal(ds.Tables["PSourceCode"].Columns.Count - 1);
                ds.Tables["PSourceCode"].Columns["c"].SetOrdinal(ds.Tables["PSourceCode"].Columns.Count - 1);
                ds.Tables["PSourceCode"].Columns["a"].SetOrdinal(ds.Tables["PSourceCode"].Columns.Count - 1);
                ds.Tables["PSourceCode"].Columns["unnecessary"].SetOrdinal(ds.Tables["PSourceCode"].Columns.Count - 1);

                foreach (string al in split_al)
                {

                    ds.Tables["PSourceCode"].Columns[al].SetOrdinal(ds.Tables["PSourceCode"].Columns.Count - 1);
                    ds.Tables["PSourceCode"].Columns[al + "_Rank"].SetOrdinal(ds.Tables["PSourceCode"].Columns.Count - 1);


                }
            }
            double numerator, Ldenominator, Rdenominator;
            double folumla = 0.0f;
            double susp = 0.0f;
            double health = 0.0f;
            double temp1 = 0.0f;
            int NumberofPassTC = 10;
            int counter;


            for (int row = Prow_last - 1; row > 0; row--)
            {
                d = b = c = a = 0;
                numerator = 0;
                Ldenominator = 0;
                Rdenominator = 0;
                susp = 0.0f;
                health = 0.0f;
                temp1 = 0.0f;
                counter = 0;

                for (int col = Pcol_last - 1; col > 2; col--)
                {

                    mark = ds.Tables["PSourceCode"].Rows[row][col].ToString();
                    result = ds.Tables["PSourceCode"].Rows[Prow_last - 1][col].ToString();

                    if (result.Equals("PASS"))
                    {
                        if (mark.Equals("1"))
                        { //●
                            c++;
                        }
                        else if (mark.Equals("0"))
                        {
                            d++;
                        }
                        else
                            blank++;
                    }
                    else if (result.Equals("FAIL"))
                    {
                        if (mark.Equals("1"))
                        {
                            //●
                            a++;
                        }
                        else if (mark.Equals("0"))
                        {
                            b++;
                        }

                        else
                            blank++;
                    }

                    ds.Tables["PSourceCode"].Rows[row]["d"] = d;
                    ds.Tables["PSourceCode"].Rows[row]["b"] = b;
                    ds.Tables["PSourceCode"].Rows[row]["c"] = c;
                    ds.Tables["PSourceCode"].Rows[row]["a"] = a;




                }

                if (a != 0)
                {
                    foreach (string al in split_al)
                    {

                        if (al == "Tarantula")
                        {
                            try
                            {
                                if ((a + b) == 0)
                                    Ldenominator = numerator = 0.0f;
                                else
                                    Ldenominator = numerator = (double)a / (a + b);
                                if ((c + d) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = (double)c / (c + d);

                                if ((Ldenominator + Rdenominator) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (double)numerator / (Ldenominator + Rdenominator);


                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {


                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;

                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }






                        //if (ds.Tables["PSourceCode"].Rows[row][al].ToString().Equals(0) || ds.Tables["PSourceCode"].Rows[row][al].ToString().Equals(""))
                        //{
                        //    Console.WriteLine(ds.Tables["PSourceCode"].Rows[row][al].ToString());
                        //    string temp10 = ds.Tables["PSourceCode"].Rows[row][al].ToString();
                        //    ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);
                        //}
                        //else
                        //{
                        //    string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();
                        //    double temp3 = double.Parse(temp2);
                        //    folumla = folumla + temp3;
                        //    ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);
                        // }





                        //    if (ds.Tables["PSourceCode"].Rows[row][al].ToString().Equals("0") || ds.Tables["PSourceCode"].Rows[row][al].ToString().Equals(" ") || ds.Tables["PSourceCode"].Rows[row][al].ToString().Equals(""))
                        //    {
                        //    }
                        //    else
                        //    {


                        //        ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                        //    }
                        //    if (d == 0 && b == 0 && c == 0 && a == 0)
                        //    {
                        //        ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                        //    }

                        //    if (ds.Tables["PSourceCode"].Rows[row]["unnecessary"].ToString() != "1")
                        //    {

                        //        if (ds.Tables["PSourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //            ds.Tables["PSourceCode"].Rows[row][al] = " ";

                        //        else
                        //        {

                        //          //  Tarantula_formula = Tarantula_formula / failcount;
                        //            ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);
                        //        }
                        //    }

                        //}


                        //if (algorithm.Contains("Tarantula"))
                        //{
                        //    try
                        //    {
                        //        if ((a + b) == 0)
                        //            Ldenominator = numerator = 0.0f;
                        //        else
                        //            Ldenominator = numerator = (double)a / (a + b);
                        //        if ((c + d) == 0)
                        //            Rdenominator = 0.0f;
                        //        else
                        //            Rdenominator = (double)c / (c + d);

                        //        if ((Ldenominator + Rdenominator) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)numerator / (Ldenominator + Rdenominator);


                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }

                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Tarantula"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Tarantula"] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);
                        //}


                        if (al == "AMPLE")
                        {
                            try
                            {
                                if ((b + a) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = (double)a / (b + a);
                                if ((d + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = (double)c / (d + c);

                                folumla = Math.Abs(Ldenominator - Rdenominator);

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }


                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }


                        if (al == "Jaccard")
                        {

                            try
                            {
                                if ((a + b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (double)a / (a + b + c);

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }


                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }



                        if (al == "SEM1")
                        {
                            try
                            {
                                if ((b + a) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = (double)a / (b + a);
                                if ((d + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = (double)c / (d + c);

                                folumla = Ldenominator * (Math.Abs(Ldenominator - Rdenominator));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }


                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "SEM2")
                        {
                            try
                            {
                                if ((b + a) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = (double)a / (b + a);
                                if ((d + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = (double)c / (d + c);

                                folumla = Ldenominator - Rdenominator;

                                if (folumla <= 0)
                                {
                                    folumla = 0.0f; ;
                                }

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }


                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }



                        if (al == "SEM3")
                        {
                            try
                            {
                                if ((b + a) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = (double)a / (b + a);
                                if ((d + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = (double)c / (d + c);

                                folumla = Ldenominator * (Ldenominator - Rdenominator);

                                if (folumla <= 0 || a == 0)
                                {
                                    folumla = 0.0f; ;
                                }

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }


                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }


                        if (al == "Naish")
                        {
                            try
                            {
                                folumla = a - (c / ((c + d) + 1));


                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "Ochiai")
                        {
                            try
                            {
                                if ((Math.Sqrt((a + b) * (a + c))) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / (Math.Sqrt((a + b) * (a + c)));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }
                        if (al == "Zoltar")
                        {
                            try
                            {
                                if ((double)(a + b + c + ((10000 * a * c) / c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (double)a / (a + b + c + ((10000 * a * c) / c));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "Kulczynski2")
                        {
                            try
                            {

                                if ((b + a) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = (double)a / (b + a);

                                if ((a + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = (double)a / (a + c);

                                folumla = (double)(Ldenominator + Rdenominator) / 2;
                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }
                        if (al == "Anderberg")
                        {
                            try
                            {
                                if ((a + 2 * (b + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (double)a / (a + 2 * (b + c));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "M2")
                        {
                            try
                            {
                                if ((a + d + 2 * (b + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (double)a / (a + d + 2 * (b + c));
                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "Dice")
                        {
                            try
                            {
                                if ((a + b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (double)(2 * a) / (a + b + c);

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "PS")
                        {
                            try
                            {



                                if (((a + c + b + d)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = -((4 * c * b) / ((a + c + b + d) * (a + c + b + d)));



                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "Wong3")
                        {
                            try
                            {
                                if (c <= 2)
                                {
                                    folumla = (double)(a - c);
                                }
                                else if (2 < c && c <= 10)
                                {
                                    folumla = (double)a - (2 + (0.1 * (c - 2)));
                                }
                                else if (c > 10)
                                {
                                    folumla = (double)a - (2.8 + (0.001 * (c - 10)));
                                }
                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "GP08")
                        {
                            try
                            {
                                folumla = a * a * ((2 * c) + (2 * a) + (3 * d));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "GP10")
                        {
                            try
                            {
                                folumla = Math.Sqrt(Math.Abs(a - (1 / d)));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }
                        if (al == "GP11")
                        {
                            try
                            {
                                folumla = (a * a) * ((a * a) + (Math.Sqrt(d)));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "GP13")
                        {
                            try
                            {
                                folumla = a * (1 + (1 / ((2 * c) + a)));


                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "GP20")
                        {
                            try
                            {
                                if ((c + d) == 0)
                                    folumla = 2 * (a + 0);
                                else
                                    folumla = 2 * (a + (d / (c + d)));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }

                        if (al == "GP26")
                        {
                            try
                            {
                                folumla = (2 * a) + (Math.Sqrt(d));


                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (d == 0 && b == 0 && c == 0 && a == 0)
                            {
                                ds.Tables["PSourceCode"].Rows[row]["unnecessary"] = 1;
                                ds.Tables["PSourceCode"].Rows[row][al] = " ";
                            }

                            else
                            {
                                string temp2 = ds.Tables["PSourceCode"].Rows[row][al].ToString();

                                if (temp2 == "")
                                {
                                    temp2 = "0";
                                }
                                double temp3 = double.Parse(temp2);
                                folumla = folumla + temp3;
                                ds.Tables["PSourceCode"].Rows[row][al] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                            }

                        }


                    }//for 

                }







            }//for
        }




        void DivideFP(DataSet dsF, DataSet dsP)
        {
            //int col_TC_ID = ds.Tables["SourceCode"].Columns["SourceCode"].Ordinal + 1;
            //int row_last = ds.Tables["SourceCode"].Rows.Count;
            //int col_last = ds.Tables["SourceCode"].Columns.Count;

            int Fcol_TC_ID = dsF.Tables["FSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Frow_last = dsF.Tables["FSourceCode"].Rows.Count;
            int Fcol_last = dsF.Tables["FSourceCode"].Columns.Count;

            int Pcol_TC_ID = dsP.Tables["PSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Prow_last = dsP.Tables["PSourceCode"].Rows.Count;
            int Pcol_last = dsP.Tables["PSourceCode"].Columns.Count;

            string Fresult;
            string Presult;


            int[] fail = new int[failcount];
            int[] pass = new int[passcount];

            int RandomNF = 0;
            int RandomNP = 0;

            // int temp1 = Fcol_last;

            for (int col = Fcol_TC_ID; col < Fcol_last; col++)
            {
                Fresult = dsF.Tables["FSourceCode"].Rows[Frow_last - 1][col].ToString();

                if (Fresult.Equals("PASS") || Fresult.Equals(""))
                {

                    dsF.Tables["FSourceCode"].Columns.RemoveAt(col);
                    col = col - 1;
                    Fcol_last = Fcol_last - 1;
                }
                //else if (Fresult.Equals("FAIL") && dsF.Tables["FSourceCode"].Columns.Count - Fcol_TC_ID > 6)
                //{
                //    RandomNF = FNum.Next(0, 2);
                //    if (RandomNF == 0)
                //    {
                //        dsF.Tables["FSourceCode"].Columns.RemoveAt(col);
                //        col = col - 1;
                //        Fcol_last = Fcol_last - 1;
                //    }

                //}

            }


            failcount = Fcol_last - Fcol_TC_ID;




            // int temp2 = Pcol_last;
            for (int col = Pcol_TC_ID; col < Pcol_last; col++)
            {
                Presult = dsP.Tables["PSourceCode"].Rows[Prow_last - 1][col].ToString();

                if (Presult.Equals("FAIL") || Presult.Equals(""))
                {
                    dsP.Tables["PSourceCode"].Columns.RemoveAt(col);
                    // dsP.Tables["PSourceCode"].Rows[col].Delete();
                    col = col - 1;
                    Pcol_last = Pcol_last - 1;
                }
                //else if (Presult.Equals("PASS") && dsP.Tables["PSourceCode"].Columns.Count - Pcol_TC_ID > 6)
                //{
                //    RandomNP = PNum.Next(0, 2);
                //    if (RandomNP == 0)
                //    {
                //        dsP.Tables["PSourceCode"].Columns.RemoveAt(col);
                //        col = col - 1;
                //        Pcol_last = Pcol_last - 1;

                //    }

                //}

            }

            //if (Pcol_last - Pcol_TC_ID > 10)
            //{
            //    int temp2 = Pcol_last;
            //    for (int col = Pcol_TC_ID; col < Pcol_last/1.4; col++)
            //    {
            //        if (Fcol_last - Fcol_TC_ID > 10)
            //        {
            //            RandomNP = PNum.Next(Pcol_TC_ID, temp2);
            //            Console.WriteLine(RandomNP);
            //            dsP.Tables["PSourceCode"].Columns.RemoveAt(RandomNP);
            //            temp2 = temp2 - 1;
            //        }
            //        else break;
            //    }

            //}



        }
        void BrayCurtis(DataSet dsF, DataSet dsP)
        {
            int Fcol_TC_ID = dsF.Tables["FSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Frow_last = dsF.Tables["FSourceCode"].Rows.Count;
            int Fcol_last = dsF.Tables["FSourceCode"].Columns.Count;

            int Pcol_TC_ID = dsP.Tables["PSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Prow_last = dsP.Tables["PSourceCode"].Rows.Count;
            int Pcol_last = dsP.Tables["PSourceCode"].Columns.Count;


            string Fmark;
            string Pmark;
            double Fcount = 0;
            double Pcount = 0;
            double count = 0;
            double BrayCurtis = 0;
            //  ArrayList HD = new ArrayList();
            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');


            for (int Fcol = Fcol_TC_ID; Fcol < Fcol_last; Fcol++)
            {
                try
                {
                    dsP.Tables["PSourceCode"].Columns.Remove("d");
                    dsP.Tables["PSourceCode"].Columns.Remove("b");
                    dsP.Tables["PSourceCode"].Columns.Remove("c");
                    dsP.Tables["PSourceCode"].Columns.Remove("a");
                    dsP.Tables["PSourceCode"].Columns.Remove("unnecessary");

                    foreach (string al in split_al)
                    {
                        dsP.Tables["PSourceCode"].Columns.Remove(al);
                        dsP.Tables["PSourceCode"].Columns.Remove(al + "_Rank");

                    }

                }
                catch
                {
                }
                finally
                {
                    for (int Pcol = Pcol_TC_ID; Pcol < Pcol_last; Pcol++)
                    {

                        for (int row = 1; row < Prow_last - 1; row++)
                        {
                            Fmark = dsF.Tables["FSourceCode"].Rows[row][Fcol].ToString();
                            Pmark = dsP.Tables["PSourceCode"].Rows[row][Pcol].ToString();

                            if (Fmark.Equals("1"))
                            {
                                Fcount = Fcount + 1;
                                if (Pmark.Equals("1"))
                                {
                                    Pcount = Pcount + 1;
                                }
                                else if (Pmark.Equals("0"))
                                {
                                    count = count + 1;
                                }
                            }
                            else if (Fmark.Equals("0"))
                            {

                                if (Pmark.Equals("1"))
                                {
                                    count = count + 1;
                                    Pcount = Pcount + 1;
                                }
                                else if (Pmark.Equals("0"))
                                {

                                }
                            }



                        }

                        BrayCurtis = (count / (Pcount + Fcount)) * 1000;
                        // BrayCurtis = Math.Round(BrayCurtis);
                        dsP.Tables["PSourceCode"].Rows[1][Pcol] = string.Format("{0:0000.0000}", BrayCurtis);



                        count = 0;
                        Fcount = 0;
                        Pcount = 0;
                        BrayCurtis = 0;
                    }
                    Sorting(dsP);
                    Preprocess(dsF, dsP);
                }
            }
        }



        void HammingDistance(DataSet dsF, DataSet dsP)
        {

            //int col_TC_ID = ds.Tables["SourceCode"].Columns["SourceCode"].Ordinal + 1;
            //int row_last = ds.Tables["SourceCode"].Rows.Count;
            //int col_last = ds.Tables["SourceCode"].Columns.Count;

            int Fcol_TC_ID = dsF.Tables["FSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Frow_last = dsF.Tables["FSourceCode"].Rows.Count;
            int Fcol_last = dsF.Tables["FSourceCode"].Columns.Count;

            int Pcol_TC_ID = dsP.Tables["PSourceCode"].Columns["SourceCode"].Ordinal + 1;
            int Prow_last = dsP.Tables["PSourceCode"].Rows.Count;
            int Pcol_last = dsP.Tables["PSourceCode"].Columns.Count;


            string Fmark;
            string Pmark;
            int count = 0;
            ArrayList HD = new ArrayList();
            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');


            for (int Fcol = Fcol_TC_ID; Fcol < Fcol_last; Fcol++)
            {


                try
                {
                    dsP.Tables["PSourceCode"].Columns.Remove("d");
                    dsP.Tables["PSourceCode"].Columns.Remove("b");
                    dsP.Tables["PSourceCode"].Columns.Remove("c");
                    dsP.Tables["PSourceCode"].Columns.Remove("a");
                    dsP.Tables["PSourceCode"].Columns.Remove("unnecessary");


                    foreach (string al in split_al)
                    {

                        dsP.Tables["PSourceCode"].Columns.Remove(al);
                        dsP.Tables["PSourceCode"].Columns.Remove(al + "_Rank");

                    }

                }
                catch
                {



                }

                finally
                {
                    for (int Pcol = Pcol_TC_ID; Pcol < Pcol_last; Pcol++)
                    {

                        for (int row = 1; row < Prow_last - 1; row++)
                        {
                            Fmark = dsF.Tables["FSourceCode"].Rows[row][Fcol].ToString();
                            Pmark = dsP.Tables["PSourceCode"].Rows[row][Pcol].ToString();

                            if (Fmark.Equals(Pmark))
                            {

                            }
                            else
                            {
                                count++;

                            }
                            dsP.Tables["PSourceCode"].Rows[0][Pcol] = string.Format("{0:0000}", count);

                        }
                        count = 0;
                    }
                    Sorting(dsP);
                    Preprocess(dsF, dsP);

                }




            }





        }

        void ComputeSuspiciousValue(DataSet ds)
        {

            int col_TC_ID = ds.Tables["SourceCode"].Columns["SourceCode"].Ordinal + 1;
            int row_last = ds.Tables["SourceCode"].Rows.Count;
            int col_last = ds.Tables["SourceCode"].Columns.Count;
            double d, b, c, a, blank = 0;



            string result;
            string mark;

            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {
                ds.Tables["SourceCode"].Columns.Add("d");
                ds.Tables["SourceCode"].Columns.Add("b");
                ds.Tables["SourceCode"].Columns.Add("c");
                ds.Tables["SourceCode"].Columns.Add("a");
                ds.Tables["SourceCode"].Columns.Add("unnecessary");


                foreach (string al in split_al)
                {
                    ds.Tables["SourceCode"].Columns.Add(al, typeof(string));
                    ds.Tables["SourceCode"].Columns.Add(al + "_Rank", typeof(string));
                }

            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                ds.Tables["SourceCode"].Columns["d"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                ds.Tables["SourceCode"].Columns["b"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                ds.Tables["SourceCode"].Columns["c"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                ds.Tables["SourceCode"].Columns["a"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                ds.Tables["SourceCode"].Columns["unnecessary"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);

                foreach (string al in split_al)
                {
                    ds.Tables["SourceCode"].Columns[al].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                    ds.Tables["SourceCode"].Columns[al + "_Rank"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                }
            }

            double numerator, Ldenominator, Rdenominator, temp1, temp2;
            double folumla = 0.0f;



            for (int row = 1; row < row_last - 1; row++)
            {
                d = b = c = a = 0;
                int PassTCcount = 0;
                int FailTCcount = 0;


                for (int col = col_TC_ID; col < col_last; col++)
                {
                    mark = ds.Tables["SourceCode"].Rows[row][col].ToString();
                    //      Console.WriteLine(mark);
                    //      Logger.WriteLine(mark);

                    result = ds.Tables["SourceCode"].Rows[row_last - 1][col].ToString();

                    if (result.Equals("PASS"))
                    {
                        PassTCcount = PassTCcount + 1;
                        if (mark.Equals(@"1"))
                        {
                            c++;
                        }
                    }

                    else if (result.Equals("FAIL"))
                    {
                        FailTCcount = FailTCcount + 1;
                        if (mark.Equals(@"1")) //●
                            a++;
                    }
                }
                d = PassTCcount - c;
                b = FailTCcount - a;

                if (a == 0 && c == 0)
                {
                    d = 0;
                    b = 0;
                }

                ds.Tables["SourceCode"].Rows[row]["d"] = d;
                ds.Tables["SourceCode"].Rows[row]["b"] = b;
                ds.Tables["SourceCode"].Rows[row]["c"] = c;
                ds.Tables["SourceCode"].Rows[row]["a"] = a;

                if (d == 0 && b == 0 && c == 0 && a == 0)
                {
                    ds.Tables["SourceCode"].Rows[row]["unnecessary"] = 1;
                }

                else
                {

                    foreach (string al in split_al)
                    {


                        if (al == "TARANTULA")
                        {
                            try
                            {
                                if ((a + b) == 0)
                                    Ldenominator = numerator = 0.0f;
                                else
                                    Ldenominator = numerator = a / (a + b);
                                if ((c + d) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = c / (c + d);

                                if ((Ldenominator + Rdenominator) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = numerator / (Ldenominator + Rdenominator);


                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["TARANTULA"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["TARANTULA"] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);
                        }



                        if (al =="AMPLE")
                        {
                            try
                            {
                                if ((b + a) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = a / (b + a);
                                if ((d + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = c / (d + c);

                                folumla = Math.Abs(Ldenominator - Rdenominator);

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["AMPLE"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["AMPLE"] = folumla.ToString("#0.#####");


                        }

                        if (al =="Jaccard")
                        {
                            try
                            {
                                if ((a + b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (double)a / (a + b + c);

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["Jaccard"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["Jaccard"] = folumla.ToString("#0.#####");
                        }


                        if (al =="Dice")
                        {

                            try
                            {
                                if ((a + b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = 2 * a / (2 * a + b + c);

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["Dice"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["Dice"] = folumla.ToString("#0.#####");


                        }
                        if (al =="CZEKANOWSKI")
                        {

                            try
                            {
                                if ((2 * a + b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = 2 * a / (2 * a + b + c);

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["CZEKANOWSKI"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["CZEKANOWSKI"] = folumla.ToString("#0.#####");


                        }
                        if (al =="_3WJACCARD")
                        {
                            try
                            {
                                if ((3 * a + b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = 3 * a / (3 * a + b + c);
                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["_3WJACCARD"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["_3WJACCARD"] = folumla.ToString("#0.#####");
                        }
                        if (al =="NEIandLI")
                        {
                            try
                            {
                                if (((a + b) + (a + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = 2 * a / ((a + b) + (a + c));
                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["NEIandLI"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["NEIandLI"] = folumla.ToString("#0.#####");
                        }
                        if (al =="SOKALandSNEATH_1")
                        {
                            try
                            {
                                if ((a + 2 * b + 2 * c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / (a + 2 * b + 2 * c);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SOKALandSNEATH_1"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SOKALandSNEATH_1"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="SOKALandMICHENER")
                        {
                            try
                            {
                                if ((a + b + c + d) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a + d) / (a + b + c + d);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SOKALandMICHENER"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SOKALandMICHENER"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="SOKALandSNEATH2")
                        {
                            try
                            {
                                if ((2 * a + b + c + 2 * d) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = 2 * (a + d) / (2 * a + b + c + 2 * d);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SOKALandSNEATH2"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SOKALandSNEATH2"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="ROGERandTANIMOTO")
                        {
                            try
                            {
                                if ((a + 2 * (b + c) + d) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a + d) / (a + 2 * (b + c) + d);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["ROGERandTANIMOTO"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["ROGERandTANIMOTO"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="FAITH")
                        {
                            try
                            {
                                if ((a + b + c + d) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a + (0.5 * d)) / (a + b + c + d);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["FAITH"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["FAITH"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="GOWERandLEGENDRE")
                        {
                            try
                            {
                                if ((a + 0.5 * (b + c) + d) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a + d) / (a + 0.5 * (b + c) + d);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["GOWERandLEGENDRE"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["GOWERandLEGENDRE"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="INTERSECTION")
                        {
                            try
                            {
                                folumla = a;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["INTERSECTION"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["INTERSECTION"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="INNERPRODUCT")
                        {
                            try
                            {
                                folumla = a + d;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["INNERPRODUCT"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["INNERPRODUCT"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="RUSSELLandRAO")
                        {
                            try
                            {
                                if ((a + b + c + d) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / (a + b + c + d);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["RUSSELLandRAO"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["RUSSELLandRAO"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="HAMMING")
                        {
                            try
                            {
                                folumla = b + c;
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["HAMMING"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["HAMMING"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="EUCLID")
                        {
                            try
                            {
                                folumla = Sqrt(b + c);
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["EUCLID"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["EUCLID"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="SQUARED_EUCLID")
                        {
                            try
                            {
                                folumla = Sqrt(Math.Pow((b + c), 2));
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SQUARED_EUCLID"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SQUARED_EUCLID"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="CANBERRA")
                        {
                            try
                            {
                                folumla = Math.Pow((b + c), 1);
                                folumla = 1 / folumla;

                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["CANBERRA"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["CANBERRA"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="MANHATTAN")
                        {
                            try
                            {
                                folumla = b + c;
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["MANHATTAN"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["MANHATTAN"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="MEAN_MANHATTAN")
                        {
                            try
                            {
                                if ((a + b + c + d) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (b + c) / (a + b + c + d);
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["MEAN_MANHATTAN"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["MEAN_MANHATTAN"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="CITYBLOCK")
                        {
                            try
                            {
                                folumla = b + c;
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["CITYBLOCK"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["CITYBLOCK"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="MINKOWSK")
                        {
                            try
                            {
                                folumla = Math.Pow((b + c), (1));
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["MINKOWSK"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["MINKOWSK"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="VARI")
                        {
                            try
                            {
                                if ((4 * (a + b + c + d)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (b + c) / 4 * (a + b + c + d);
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["VARI"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["VARI"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="SIZEDIFFERENCE")
                        {
                            try
                            {
                                if (Math.Pow((a + b + c + d), 2) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = Math.Pow((b + c), 2) / Math.Pow((a + b + c + d), 2);
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SIZEDIFFERENCE"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SIZEDIFFERENCE"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="SHAPEDIFFERENCE")
                        {
                            try
                            {
                                if (Math.Pow((a + b + c + d), 2) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a + b + c + d) * (b + c) - Math.Pow((b - c), 2) / Math.Pow((a + b + c + d), 2);
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SHAPEDIFFERENCE"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SHAPEDIFFERENCE"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="PATTERNDIFFERENCE")
                        {
                            try
                            {
                                if (Math.Pow((a + b + c + d), 2) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = 4 * b * c / Math.Pow((a + b + c + d), 2);
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["PATTERNDIFFERENCE"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["PATTERNDIFFERENCE"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="LANCEandWILLIAMS")
                        {
                            try
                            {
                                if ((2 * a + b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = b + c / (2 * a + b + c);
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["LANCEandWILLIAMS"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["LANCEandWILLIAMS"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="BRAYandCURTIS")
                        {
                            try
                            {
                                if ((2 * a + b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = b + c / (2 * a + b + c);
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["BRAYandCURTIS"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["BRAYandCURTIS"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="HELLINGER")
                        {
                            try
                            {
                                if (Sqrt((a + b) * (a + c)) == 0)
                                    folumla = 2 * Sqrt(1 - 0);
                                else
                                    folumla = 2 * Sqrt(1 - (a / Sqrt((a + b) * (a + c))));
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["HELLINGER"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["HELLINGER"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="CHORD")
                        {
                            try
                            {
                                if (Sqrt((a + b) * (a + c)) == 0)
                                    folumla = Sqrt(2 * (1 - 0));
                                else
                                    folumla = Sqrt(2 * (1 - (a / Sqrt((a + b) * (a + c)))));
                                folumla = 1 / folumla;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["CHORD"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["CHORD"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="COSINE")
                        {
                            try
                            {
                                if (Math.Pow(Sqrt((a + b) * (a + c)), 2) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / Math.Pow(Sqrt((a + b) * (a + c)), 2);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["COSINE"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["COSINE"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="GILBERTandWELLS")
                        {
                            try
                            {
                                if (a == 0)
                                {
                                    folumla = 0 - Math.Log((a + b + c + d)) - Math.Log((a + b) / (a + b + c + d)) - Math.Log((a + c) / (a + b + c + d));
                                }
                                else if ((a + b) == 0)
                                {
                                    folumla = 0.0f;
                                }
                                else if ((a + c) == 0)
                                {
                                    folumla = 0.0f;
                                }
                                else
                                {
                                    folumla = Math.Log(a) - Math.Log((a + b + c + d)) - Math.Log((a + b) / (a + b + c + d)) - Math.Log((a + c) / (a + b + c + d));
                                }

                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["GILBERTandWELLS"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["GILBERTandWELLS"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="OCHIAI1")
                        {
                            try
                            {
                                if (Sqrt((a + b) * (a + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / Sqrt((a + b) * (a + c));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["OCHIAI1"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["OCHIAI1"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="FORBESI")
                        {
                            try
                            {
                                if (((a + b) * (a + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a + b + c + d) * a / ((a + b) * (a + c));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["FORBESI"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["FORBESI"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="FOSSUM")
                        {
                            try
                            {
                                if (((a + b) * (a + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = ((a + b + c + d) * (Math.Pow((a - 0.5), 2))) / ((a + b) * (a + c));

                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["FOSSUM"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["FOSSUM"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="SORGENFREI")
                        {
                            try
                            {
                                if (((a + b) * (a + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = Math.Pow(a, 2) / ((a + b) * (a + c));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SORGENFREI"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SORGENFREI"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="MOUNTFORD")
                        {
                            try
                            {
                                if (0.5 * ((a * b) + (a * c)) + b * c == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / 0.5 * ((a * b) + (a * c)) + b * c;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["MOUNTFORD"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["MOUNTFORD"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="OTSUKA")
                        {
                            try
                            {
                                if (Math.Pow(((a + b) * (a + c)), 0.5) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / Math.Pow(((a + b) * (a + c)), 0.5);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["OTSUKA"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["OTSUKA"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="MCCONNAUGHEY")
                        {
                            try
                            {
                                if ((a + b) * (a + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (Math.Pow(a, 2) - (b * c)) / ((a + b) * (a + c));

                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["MCCONNAUGHEY"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["MCCONNAUGHEY"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="TARWID")
                        {
                            try
                            {
                                if (((a + b + c + d) * a) + (a + b) * (a + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (((a + b + c + d) * a) - (a + b) * (a + c)) / ((a + b + c + d) * a + (a + b) * (a + c));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["TARWID"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["TARWID"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="KULCZYNSK2")
                        {
                            try
                            {
                                if ((a + b) * (a + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a / 2) * (2 * a + b + c) / (a + b) * (a + c);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["KULCZYNSK2"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["KULCZYNSK2"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="DRIVERandKROEBER")
                        {
                            try
                            {

                                if ((a + b) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = 1 / (a + b);
                                if ((a + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = 1 / (a + c);

                                if ((Ldenominator + Rdenominator) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a / 2) * (Ldenominator + Rdenominator);

                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["DRIVERandKROEBER"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["DRIVERandKROEBER"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="JOHNSON")
                        {
                            try
                            {
                                if ((a + b) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = a / (a + b);
                                if ((a + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = a / (a + c);

                                if ((Ldenominator + Rdenominator) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (Ldenominator + Rdenominator);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["JOHNSON"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["JOHNSON"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="DENNIS")
                        {
                            try
                            {
                                if (((a + b + c + d) * (a + b) * (a + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a * d - b * c) / (Sqrt((a + b + c + d) * (a + b) * (a + c)));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["DENNIS"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["DENNIS"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="SIMPSON")
                        {
                            try
                            {
                                if (Math.Min((a + b), (a + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / Math.Min((a + b), (a + c));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SIMPSON"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SIMPSON"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="BRAUNandBANQUET")
                        {
                            try
                            {
                                if (Math.Max((a + b), (a + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / Math.Max((a + b), (a + c));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["BRAUNandBANQUET"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["BRAUNandBANQUET"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="FAGERandMCGOWAN")
                        {
                            try
                            {
                                if (Sqrt((a + b) * (a + c)) == 0)
                                    folumla = 0 - (Math.Max((a + b), (a + c)) / 2);
                                else
                                    folumla = (a / Sqrt((a + b) * (a + c))) - (Math.Max((a + b), (a + c)) / 2);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["FAGERandMCGOWAN"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["FAGERandMCGOWAN"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="FORBES2")
                        {
                            try
                            {
                                if ((((a + b + c + d) * Math.Min((a + b), (a + c))) - (a + b) * (a + c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (((a + b + c + d) * a) - ((a + b) * (a + c))) / (((a + b + c + d) * Math.Min((a + b), (a + c))) - (a + b) * (a + c));

                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["FORBES2"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["FORBES2"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="SOKALandSNEATH4")
                        {
                            double one = 0;
                            double two = 0;
                            double three = 0;
                            try
                            {
                                if ((a + b) == 0)
                                {
                                    one = 0;
                                }
                                else
                                {
                                    one = (a / (a + b));
                                }

                                if ((a + c) == 0)
                                {
                                    two = 0;
                                }
                                else
                                {
                                    two = (a / (a + c));
                                }

                                if ((b + d) == 0)
                                {
                                    three = 0;
                                }
                                else
                                {
                                    three = (d / (b + d));
                                }

                                folumla = (one + two + three + three) / 4;
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SOKALandSNEATH4"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SOKALandSNEATH4"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="GOWER")
                        {
                            try
                            {
                                if (((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a + d) / Sqrt((a + b) * (a + c) * (b + d) * (c + d));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["GOWER"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["GOWER"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="PEARSON1")
                        {
                            try
                            {
                                if (((a + b) * (a + c) * (c + d) * (b + d)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (((a + b + c + d) * Math.Pow((a * d - b * c), 2)) / ((a + b) * (a + c) * (c + d) * (b + d)));

                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["PEARSON1"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["PEARSON1"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="PEARSON2")
                        {
                            try
                            {
                                if (((a + b) * (a + c) * (c + d) * (b + d)) == 0)
                                    folumla = 0.0f;
                                else if ((a * d - b * c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = Math.Pow((((a + b + c + d) * Math.Pow((a * d - b * c), 2)) / ((a + b) * (a + c) * (c + d) * (b + d))) / ((a + b + c + d) + (((a + b + c + d) * Math.Pow((a * d - b * c), 2)) / ((a + b) * (a + c) * (c + d) * (b + d)))), (1 / 2));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["PEARSON2"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["PEARSON2"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="PEARSON3")
                        {

                            try
                            {
                                if (((a + b) * (a + c) * (c + d) * (b + d)) == 0)
                                {
                                    folumla = 0.0f;
                                }
                                else if (Sqrt(a + b) * (a + c) * (b + d) * (c + d) == 0)
                                {
                                    folumla = 0.0f;
                                }
                                else if ((a * d - b * c) == 0)
                                {
                                    folumla = 0.0f;
                                }
                                else
                                {
                                    if (((((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)))) / ((a + b + c + d) + (((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)))))) < 0)
                                    {
                                        double temp = -((((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)))) / ((a + b + c + d) + (((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d))))));
                                        // Logger.WriteLine("temp:" + temp);

                                        folumla = Math.Pow(temp, 0.5);
                                        // Logger.WriteLine("PEARSON3folumla:" + PEARSON3folumla);
                                        folumla = -folumla;
                                        // Logger.WriteLine("PEARSON3folumla2:" + PEARSON3folumla);
                                    }
                                    else
                                    {
                                        folumla = Math.Pow((((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d)))) / ((a + b + c + d) + (((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d))))), 0.5);
                                    }

                                }


                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["PEARSON3"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["PEARSON3"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="PEARSONandHERON1")
                        {
                            try
                            {
                                if (((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (((a * d - b * c) / Sqrt((a + b) * (a + c) * (b + d) * (c + d))));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["PEARSONandHERON1"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["PEARSONandHERON1"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="PEARSONandHERON2")
                        {
                            try
                            {
                                if ((a * d) == 0)
                                    folumla = 0.0f;
                                else if ((b * c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = Math.Cos((Math.PI * Sqrt(b * c)) / (Sqrt(a * d) + Sqrt(b * c)));

                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["PEARSONandHERON2"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["PEARSONandHERON2"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="SOKALandSNEATH3")
                        {
                            try
                            {
                                if ((b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a + d) / (b + c);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SOKALandSNEATH3"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SOKALandSNEATH3"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="SOKALandSNEATH5")
                        {
                            try
                            {
                                if (((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a * d / ((a + b) * (a + c) * (b + d) * Math.Pow((c + d), 0.5));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["SOKALandSNEATH5"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["SOKALandSNEATH5"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="COLE")
                        {
                            try
                            {
                                if ((Math.Pow((a * d - b * c), 2) - ((a + b) * (a + c) * (b + d) * (c + d))) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (Sqrt(2) * ((a * d - b * c))) / (Sqrt(Math.Pow((a * d - b * c), 2) - ((a + b) * (a + c) * (b + d) * (c + d))));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["COLE"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["COLE"] = folumla.ToString("#0.#####");
                        }


                        if (al =="STILES")
                        {
                            try
                            {
                                if (((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                    folumla = 0.0f;
                                else if (Math.Pow((Math.Abs(a * d - b * c) - ((a + b + c + d) / 2)), 2) / ((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = Math.Log10(((a + b + c + d) * Math.Pow((Math.Abs(a * d - b * c) - ((a + b + c + d) / 2)), 2)) / ((a + b) * (a + c) * (b + d) * (c + d)));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["STILES"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["STILES"] = folumla.ToString("#0.#####");
                        }


                        if (al =="OCHIAI2")
                        {
                            try
                            {
                                if ((a * d + b * c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a * d - b * c) / (a * d + b * c);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["OCHIAI2"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["OCHIAI2"] = folumla.ToString("#0.#####");
                        }


                        if (al =="YULEQ")
                        {
                            try
                            {
                                if ((a * d + b * c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a * d - b * c) / (a * d + b * c);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["YULEQ"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["YULEQ"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="D_YULEQ")
                        {
                            try
                            {
                                if ((a * d + b * c) == 0)
                                    folumla = 0.0f;
                                else if (b * c == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = 2 * b * c / (a * d + b * c);
                                folumla = 1 / folumla;

                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["D_YULEQ"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["D_YULEQ"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////



                        if (al =="YULEw")
                        {
                            try
                            {
                                if ((Sqrt(a * d) + Sqrt(b * c)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (Sqrt(a * d) - Sqrt(b * c)) / (Sqrt(a * d) + Sqrt(b * c));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["YULEw"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["YULEw"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="KULCZYNSKI1")
                        {
                            try
                            {
                                if ((b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / (b + c);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["KULCZYNSKI1"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["KULCZYNSKI1"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="TANIMOTO")
                        {
                            try
                            {
                                if (((a + b) + (a + c) - a) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = a / ((a + b) + (a + c) - a);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["TANIMOTO"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["TANIMOTO"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="DISPERSON")
                        {
                            try
                            {
                                if ((Math.Pow((a + b + c + d), 2)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a * d - b * c) / (Math.Pow((a + b + c + d), 2));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["DISPERSON"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["DISPERSON"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="HAMANN")
                        {
                            try
                            {
                                if ((a + b + c + d) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = ((a + d) - (b + c)) / (a + b + c + d);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["HAMANN"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["HAMANN"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="MICHAEL")
                        {
                            try
                            {
                                if ((Math.Pow((a + d), 2) + Math.Pow((b + c), 2)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = 4 * (a * d - b * c) / (Math.Pow((a + d), 2) + Math.Pow((b + c), 2));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["MICHAEL"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["MICHAEL"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="GOODMANandKRUSKAL")
                        {
                            try
                            {
                                if ((2 * (a + b + c + d) - (Math.Max(a + c, b + d) + Math.Max(a + b, c + d))) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = ((Math.Max(a, b) + Math.Max(c, d) + Math.Max(a, c) + Math.Max(b, d)) - (Math.Max(a + c, b + d) + Math.Max(a + b, c + d))) / (2 * (a + b + c + d) - (Math.Max(a + c, b + d) + Math.Max(a + b, c + d)));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["GOODMANandKRUSKAL"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["GOODMANandKRUSKAL"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="ANDERBERG")
                        {
                            try
                            {
                                if ((2 * (a + b + c + d)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = ((Math.Max(a, b) + Math.Max(c, d) + Math.Max(a, c) + Math.Max(b, d)) - (Math.Max(a + c, b + d) + Math.Max(a + b, c + d))) / (2 * (a + b + c + d));

                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["ANDERBERG"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["ANDERBERG"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="BARONI_URBANIandBUSER1")
                        {
                            try
                            {
                                if ((Sqrt(a * d) + a + b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (Sqrt(a * d) + a) / (Sqrt(a * d) + a + b + c);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["BARONI_URBANIandBUSER1"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["BARONI_URBANIandBUSER1"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

                        if (al =="BARONI_URBANIandBUSER2")
                        {
                            try
                            {
                                if ((Sqrt(a * d) + a + b + c) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (Sqrt(a * d) + a - (b + c)) / (Sqrt(a * d) + a + b + c);
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["BARONI_URBANIandBUSER2"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["BARONI_URBANIandBUSER2"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="PEIRCE")
                        {
                            try
                            {
                                if (((a * b) + (2 * b * c) + (c * d)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (a * b + b * c) / ((a * b) + (2 * b * c) + (c * d));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["PEIRCE"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["PEIRCE"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////


                        if (al =="EYRAUD")
                        {
                            try
                            {
                                if (((a + b) * (a + c) * (b + d) * (c + d)) == 0)
                                    folumla = 0.0f;
                                else
                                    folumla = (Math.Pow((a + b + c + d), 2) * ((a + b + c + d) * a - (a + b) * (a + c))) / ((a + b) * (a + c) * (b + d) * (c + d));
                            }
                            catch (System.Exception ex) { folumla = 0.0f; }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1") ds.Tables["SourceCode"].Rows[row]["EYRAUD"] = " ";
                            else ds.Tables["SourceCode"].Rows[row]["EYRAUD"] = folumla.ToString("#0.#####");
                        }
                        ////////////////////////////////////////////////////////////////////////////////////////////////////////////





                        //algorithms

                        //if (al =="Tarantula")
                        //{
                        //    try
                        //    {
                        //        if ((a + b) == 0)
                        //            Ldenominator = numerator = 0.0f;
                        //        else
                        //            Ldenominator = numerator = (double)a / (a + b);
                        //        if ((c + d) == 0)
                        //            Rdenominator = 0.0f;
                        //        else
                        //            Rdenominator = (double)c / (c + d);

                        //        if ((Ldenominator + Rdenominator) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)numerator / (Ldenominator + Rdenominator);


                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }

                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Tarantula"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Tarantula"] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);
                        //}



                        //if (al =="AMPLE")
                        //{
                        //    try
                        //    {
                        //        if ((b + a) == 0)
                        //            Ldenominator = 0.0f;
                        //        else
                        //            Ldenominator = (double)a / (b + a);
                        //        if ((d + c) == 0)
                        //            Rdenominator = 0.0f;
                        //        else
                        //            Rdenominator = (double)c / (d + c);

                        //        folumla = Math.Abs(Ldenominator - Rdenominator);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["AMPLE"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["AMPLE"] = folumla.ToString("#0.#####");


                        //}


                        //if (al =="Jaccard")
                        //{

                        //    try
                        //    {
                        //        if ((a + b + c) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)a / (a + b + c);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }

                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Jaccard"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Jaccard"] = folumla.ToString("#0.#####");


                        //}




                        if (al =="SEM1")
                        {
                            try
                            {


                                if ((b + a) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = (double)a / (b + a);
                                if ((d + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = (double)c / (d + c);
                                if ((a + c) == 0)
                                    temp1 = 0.0f;
                                else
                                    temp1 = a / (a + c);

                                folumla = temp1 * (Math.Abs(Ldenominator - Rdenominator));


                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["SEM1"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["SEM1"] = folumla.ToString("#0.#####");
                        }


                        if (al =="SEM2")

                        {
                            try
                            {
                                if ((b + a) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = (double)a / (b + a);
                                if ((d + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = (double)c / (d + c);

                                folumla = Ldenominator - Rdenominator;

                                if (folumla <= 0)
                                {
                                    folumla = 0.0f;
                                }

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["SEM2"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["SEM2"] = folumla.ToString("#0.#####");
                        }

                        if (al =="SEM3")
                        {
                            try
                            {

                                if ((b + a) == 0)
                                    Ldenominator = 0.0f;
                                else
                                    Ldenominator = (double)a / (b + a);
                                if ((d + c) == 0)
                                    Rdenominator = 0.0f;
                                else
                                    Rdenominator = (double)c / (d + c);
                                if ((a + c) == 0)
                                    temp2 = 0.0f;
                                else
                                    temp2 = a / (a + c);


                                folumla = temp2 * (Ldenominator - Rdenominator);


                                if (folumla <= 0 || a == 0)
                                {
                                    folumla = 0.0f;
                                }

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["SEM3"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["SEM3"] = folumla.ToString("#0.#####");
                        }
                        if (al =="Naish")

                        {
                            try
                            {

                                folumla = (double)a - ((double)c / (c + d + 1));


                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["Naish"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["Naish"] = folumla.ToString("#0.#####"); //(double)Math.Round(folumla, 5);


                        }
                        //if (al =="Ochiai")

                        //{
                        //    try
                        //    {
                        //        if ((Math.Sqrt((a + b) * (a + c))) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)a / Math.Sqrt((a + b) * (a + c));

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Ochiai"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Ochiai"] = folumla.ToString("#0.#####");
                        //}

                        //if (al =="Zoltar")

                        //{
                        //    try
                        //    {

                        //        if ((double)(a + b + c + ((10000 * b * c) / a)) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)a / (a + b + c + ((10000 * b * c) / a));



                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }

                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Zoltar"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Zoltar"] = folumla.ToString("#0.#####");
                        //}
                        //if (al =="Kulczynski2")

                        //{
                        //    try
                        //    {

                        //        if ((b + a) == 0)
                        //            Ldenominator = 0.0f;
                        //        else
                        //            Ldenominator = (double)a / (b + a);

                        //        if ((a + c) == 0)
                        //            Rdenominator = 0.0f;
                        //        else
                        //            Rdenominator = (double)a / (a + c);

                        //        folumla = (double)(Ldenominator + Rdenominator) / 2;
                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }

                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Kulczynski2"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Kulczynski2"] = folumla.ToString("#0.#####");
                        //}

                        //if (al =="Anderberg")

                        //{
                        //    try
                        //    {
                        //        if ((a + 2 * (b + c)) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)a / (a + 2 * (b + c));

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }

                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Anderberg"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Anderberg"] = folumla.ToString("#0.#####");
                        //}

                        //if (al =="M1")

                        //{
                        //    try
                        //    {

                        //        if ((b + c) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(a + d) / (b + c);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["M1"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["M1"] = folumla.ToString("#0.#####");
                        //}

                        //if (al =="M2")

                        //{
                        //    try
                        //    {
                        //        if ((a + d + 2 * (b + c)) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)a / (a + d + 2 * (b + c));
                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["M2"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["M2"] = folumla.ToString("#0.#####");
                        //}
                        //if (al =="Dice")

                        //{
                        //    try
                        //    {
                        //        if ((a + b + c) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(2 * a) / (a + b + c);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Dice"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Dice"] = folumla.ToString("#0.#####");
                        //}
                        //if (al =="PS")

                        //{
                        //    try
                        //    {

                        //        if (((a + c + b + d)) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)-((4 * c * b) / ((a + c + b + d) * (a + c + b + d)));
                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["PS"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["PS"] = folumla.ToString("#0.#####");
                        //}

                        //if (al =="Wong1")

                        //{
                        //    try
                        //    {
                        //        folumla = a;


                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Wong1"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Wong1"] = folumla.ToString("#0.#####");
                        //}

                        if (al =="Wong2")

                        {
                            try
                            {

                                folumla = (double)a - c;

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["Wong2"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["Wong2"] = folumla.ToString("#0.#####");
                        }

                        if (al =="Wong3")

                        {
                            try
                            {
                                if (c <= 2)
                                {
                                    folumla = (double)(a - c);
                                }
                                else if (2 < c && c <= 10)
                                {
                                    folumla = (double)a - (2 + (0.1 * (c - 2)));
                                }
                                else if (c > 10)
                                {
                                    folumla = (double)a - (2.8 + (0.001 * (c - 10)));
                                }
                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["Wong3"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["Wong3"] = folumla.ToString("#0.#####");
                        }


                        if (al =="GP08")

                        {
                            try
                            {
                                folumla = (double)a * a * ((2 * c) + (2 * a) + (3 * d));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }

                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["GP08"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["GP08"] = folumla.ToString("#0.#####");
                        }
                        if (al =="GP10")

                        {
                            try
                            {
                                folumla = (double)Math.Sqrt(Math.Abs(a - (1 / d)));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["GP10"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["GP10"] = folumla.ToString("#0.#####");
                        }

                        if (al =="GP11")

                        {
                            try
                            {
                                folumla = (double)(a * a) * ((a * a) + (Math.Sqrt(d)));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["GP11"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["GP11"] = folumla.ToString("#0.#####");
                        }
                        if (al =="GP13")

                        {
                            try
                            {
                                folumla = (double)a * (1 + (1 / ((2 * c) + a)));


                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["GP13"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["GP13"] = folumla.ToString("#0.#####");
                        }

                        if (al =="GP20")

                        {
                            try
                            {
                                if ((c + d) == 0)
                                    folumla = (double)2 * (a + 0);
                                else
                                    folumla = (double)2 * (a + (d / (c + d)));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["GP20"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["GP20"] = folumla.ToString("#0.#####");
                        }

                        if (al =="GP26")

                        {
                            try
                            {

                                folumla = (double)(2 * a) * (2 * a) + (Math.Sqrt(d));

                            }
                            catch (System.Exception ex)
                            {
                                folumla = 0.0f;
                            }
                            if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                                ds.Tables["SourceCode"].Rows[row]["GP26"] = " ";
                            else
                                ds.Tables["SourceCode"].Rows[row]["GP26"] = folumla.ToString("#0.#####");
                        }


                        //if (al =="SorensenDice")

                        //{
                        //    try
                        //    {
                        //        if ((2 * a + b + c) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)2 * a / (2 * a + b + c);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["SorensenDice"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["SorensenDice"] = folumla.ToString("#0.#####");
                        //}







                        //if (al =="SimpleMatching")

                        //{
                        //    try
                        //    {

                        //        if ((a + b + c + d) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(a + d) / (a + b + c + d);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["SimpleMatching"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["SimpleMatching"] = folumla.ToString("#0.#####");
                        //}

                        //if (al =="Sokal")

                        //{
                        //    try
                        //    {

                        //        if ((2 * (a + d) + b + c) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)2 * (a + d) / (2 * (a + d) + b + c);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Sokal"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Sokal"] = folumla.ToString("#0.#####");
                        //}
                        //if (al =="RogersTanimoto")

                        //{
                        //    try
                        //    {

                        //        if ((a + d + 2 * (b + c) == 0))
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(a + d) / (a + d + 2 * (b + c));

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["RogersTanimoto"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["RogersTanimoto"] = folumla.ToString("#0.#####");
                        //}

                        //if (al =="Goodman")
                        //{
                        //    try
                        //    {

                        //        if ((2 * a + b + c) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(2 * a - b - c) / (2 * a + b + c);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Goodman"] = " ";
                        //    else
                        //    {
                        //        folumla = folumla + 1;
                        //        ds.Tables["SourceCode"].Rows[row]["Goodman"] = folumla.ToString("#0.#####");
                        //    }

                        //}


                        //if (al =="Hammingetc")

                        //{
                        //    try
                        //    {

                        //        folumla = (double)a + d;

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Hammingetc"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Hammingetc"] = folumla.ToString("#0.#####");
                        //}


                        //if (al =="Euclid")
                        //{
                        //    try
                        //    {
                        //        folumla = (double)Math.Sqrt(a + d);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Euclid"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Euclid"] = folumla.ToString("#0.#####");
                        //}





                        //if (al =="Hamann")
                        //{
                        //    try
                        //    {

                        //        if ((a + b + c + d) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(a + d - b - c) / (a + b + c + d);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Hamann"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Hamann"] = folumla.ToString("#0.#####");
                        //}




                        //if (al =="RussellandRao")
                        //{
                        //    try
                        //    {

                        //        if ((a + b + c + d) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)a / (a + b + c + d);

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["RussellandRao"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["RussellandRao"] = folumla.ToString("#0.#####");
                        //}
                        //if (al =="Cohen")
                        //{
                        //    try
                        //    {

                        //        if ((((a + c) * (d + c)) + ((a + b) * (b + d))) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(2 * a * d) - (2 * b * c) / (((a + c) * (d + c)) + ((a + b) * (b + d)));

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Cohen"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Cohen"] = folumla.ToString("#0.#####");
                        //}


                        //if (al =="GeometricMean")

                        //{
                        //    try
                        //    {

                        //        if (Math.Sqrt((a + c) * (d + b) * (a + b) * (c + d)) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(a * d) - (b * c) / Math.Sqrt((a + c) * (d + b) * (a + b) * (c + d));

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["GeometricMean"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["GeometricMean"] = folumla.ToString("#0.#####");
                        //}

                        //if (al =="HarmonicMean")

                        //{
                        //    try
                        //    {

                        //        if (((a + c) * (d + b) * (a + b) * (c + d)) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(a * d - b * c) * (((a + c) * (d + b) + (a + b) * (c + d))) / ((a + c) * (d + b) * (a + b) * (c + d));

                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["HarmonicMean"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["HarmonicMean"] = folumla.ToString("#0.#####");
                        //}
                        //if (al =="ArithmeticMean")
                        //{
                        //    try
                        //    {

                        //        if (((a + c) * (d + b) + (a + b) * (c + d)) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(2 * a * d - 2 * b * c) / ((a + c) * (d + b) + (a + b) * (c + d));


                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["ArithmeticMean"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["ArithmeticMean"] = folumla.ToString("#0.#####");
                        //}


                        //if (al =="Rogot1")
                        //{
                        //    try
                        //    {

                        //        if ((2 * a + b + c) == 0)
                        //            Ldenominator = 0.0f;
                        //        else
                        //            Ldenominator = (double)a / (2 * a + b + c);
                        //        if ((2 * d + b + c) == 0)
                        //            Rdenominator = 0.0f;
                        //        else
                        //            Rdenominator = (double)d / (2 * d + b + c);

                        //        if ((Ldenominator + Rdenominator) == 0)
                        //            folumla = 0.0f;
                        //        else
                        //            folumla = (double)(Ldenominator + Rdenominator) / 2;


                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        folumla = 0.0f;
                        //    }
                        //    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        //        ds.Tables["SourceCode"].Rows[row]["Rogot1"] = " ";
                        //    else
                        //        ds.Tables["SourceCode"].Rows[row]["Rogot1"] = folumla.ToString("#0.#####");
                        //}
                    }
                }
            }


        }

        void ComputeSuspiciousValue0(DataSet ds)
        {

            int col_TC_ID = ds.Tables["SourceCode"].Columns["SourceCode"].Ordinal + 1;
            int row_last = ds.Tables["SourceCode"].Rows.Count;
            int col_last = ds.Tables["SourceCode"].Columns.Count;
            double d, b, c, a, Nf, Ns, Nf1, Nf2, Nf3, Ns1, Ns2, Ns3, blank = 0;
            double ScaleFactor = 0.0f;
            double RatioFailPass = 0.0f;
            double SumOfFailGroup = 0.0f;
            double SumOfPassGroup = 0.0f;



            string result;
            string mark;
            string mark2;

            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');
            try
            {
                ds.Tables["SourceCode"].Columns.Add("d");
                ds.Tables["SourceCode"].Columns.Add("b");
                ds.Tables["SourceCode"].Columns.Add("c");
                ds.Tables["SourceCode"].Columns.Add("a");
                ds.Tables["SourceCode"].Columns.Add("unnecessary");


                /*        for (int i = 1; i < failcount+1; i++)
                        {
                              foreach (string al in split_al)
                              {

                              ds.Tables["SourceCode"].Columns.Add(al, typeof(string));
                              ds.Tables["SourceCode"].Columns.Add(al + "_Rank", typeof(string));

                                 }
                      }

                  }

      */

                foreach (string al in split_al)
                {
                    ds.Tables["SourceCode"].Columns.Add(al, typeof(string));
                    ds.Tables["SourceCode"].Columns.Add(al + "_Rank", typeof(string));
                    //col_idx_fld = ds.Tables["SourceCode"].Columns[al].Ordinal;
                    //ds.Tables["SourceCode"].Columns[al + "_Rank"].SetOrdinal(col_idx_fld);

                }

            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                ds.Tables["SourceCode"].Columns["d"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                ds.Tables["SourceCode"].Columns["b"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                ds.Tables["SourceCode"].Columns["c"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                ds.Tables["SourceCode"].Columns["a"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                //ds.Tables["SourceCode"].Columns["Nf"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                //ds.Tables["SourceCode"].Columns["Ns"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                ds.Tables["SourceCode"].Columns["unnecessary"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);


                /*   for (int i = 1; i < failcount+1; i++)
                  {
                        foreach (string al in split_al)
                        {
                            ds.Tables["SourceCode"].Columns[al].SetOrdinal(ds.Tables["SourceCode"].Columns.Count- 1);
                            ds.Tables["SourceCode"].Columns[al + "_Rank"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count- 1);
                     }
                    }
                }

                */

                foreach (string al in split_al)
                {
                    ds.Tables["SourceCode"].Columns[al].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                    ds.Tables["SourceCode"].Columns[al + "_Rank"].SetOrdinal(ds.Tables["SourceCode"].Columns.Count - 1);
                }
            }
            //int col_algorithm = ds.Tables["SourceCode"].Columns[algorithm].Ordinal;

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            double numerator, Ldenominator, Rdenominator;
            double folumla = 0.0f;
            double susp = 0.0f;
            double health = 0.0f;



            for (int row = 1; row < row_last - 1; row++)
            {
                d = b = c = a = 0;
                for (int col = col_TC_ID; col < col_last; col++)
                {




                    mark = ds.Tables["SourceCode"].Rows[row][col].ToString();
                    result = ds.Tables["SourceCode"].Rows[row_last - 1][col].ToString();
                    mark2 = ds.Tables["SourceCode"].Rows[row][2].ToString();




                    if (result.Equals("PASS"))
                    {
                        if (mark.Equals(@"1")) //●
                            c++;
                        else if (mark.Equals(@"0"))
                            d++;
                        else
                            blank++;
                    }
                    else if (result.Equals("FAIL"))
                    {
                        if (mark.Equals(@"1")) //●
                            a++;
                        else if (mark.Equals(@"0"))
                            b++;
                        else
                            blank++;
                    }





                    if (result.Equals("PASS"))
                    {
                        //fail인 TC를 찾아서 
                        if (mark2.Equals(@"1"))
                        {

                            /*
                            // Jaccard
                            if ((a + b + c) == 0)
                                susp = 0.0f;
                            else
                            {
                                susp = (double)a / (a + b + c);
                                health = 1 - susp;
                                b = b + health;
                                //   d = d - health;

                        */

                            if ((a + b) == 0)
                                Ldenominator = numerator = 0.0f;
                            else
                                Ldenominator = numerator = (double)a / (a + b);
                            if ((c + d) == 0)
                                Rdenominator = 0.0f;
                            else
                                Rdenominator = (double)c / (c + d);

                            if ((Ldenominator + Rdenominator) == 0)
                                susp = 0.0f;
                            else
                                susp = (double)numerator / (Ldenominator + Rdenominator);
                            health = 1 - susp;
                            b = b + health;
                            d = d - health;
                        }

                    }
                    else if (result.Equals("FAIL"))
                    {
                        if (mark.Equals(@"1"))
                        {

                            /*
                            // Jaccard
                                if ((a + b + c) == 0)
                                    susp = 0.0f;
                                else
                                {
                                    susp = (double)a / (a + b + c);
                                    health = 1 - susp;
                                    b = b + health;
                                    //     d = d - health;

                            */

                            if ((a + b) == 0)
                                Ldenominator = numerator = 0.0f;
                            else
                                Ldenominator = numerator = (double)a / (a + b);
                            if ((c + d) == 0)
                                Rdenominator = 0.0f;
                            else
                                Rdenominator = (double)c / (c + d);

                            if ((Ldenominator + Rdenominator) == 0)
                                susp = 0.0f;
                            else
                                susp = (double)numerator / (Ldenominator + Rdenominator);
                            health = 1 - susp;
                            b = b + health;
                            d = d - health;

                        }

                    }
                    ds.Tables["SourceCode"].Rows[row]["d"] = d;
                    ds.Tables["SourceCode"].Rows[row]["b"] = b;
                    ds.Tables["SourceCode"].Rows[row]["c"] = c;
                    ds.Tables["SourceCode"].Rows[row]["a"] = a;
                }


                try
                {

                    if ((a + b) == 0)
                        Ldenominator = numerator = 0.0f;
                    else
                        Ldenominator = numerator = (double)a / (a + b);
                    if ((c + d) == 0)
                        Rdenominator = 0.0f;
                    else
                        Rdenominator = (double)c / (c + d);

                    if ((Ldenominator + Rdenominator) == 0)
                        folumla = 0.0f;
                    else
                        folumla = (double)numerator / (Ldenominator + Rdenominator);

                }

                catch (System.Exception ex)
                {
                    folumla = 0.0f;
                }

                if (d == 0 && b == 0 && c == 0 && a == 0)
                {
                    ds.Tables["SourceCode"].Rows[row]["unnecessary"] = 1;
                }

                if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() != "1")
                {

                    if (ds.Tables["SourceCode"].Rows[row]["unnecessary"].ToString() == "1")
                        ds.Tables["SourceCode"].Rows[row]["Reinforce"] = " ";

                    else
                        ds.Tables["SourceCode"].Rows[row]["Reinforce"] = folumla.ToString("#0.#####"); //Math.Round(folumla, 5);

                }
            }

        }



        void ColoringBySuspicious(DataSet ds, double threshold1, double threshold2)
        {
            string algorithm = cmbAlgorithm.Text;
            string[] split_al = algorithm.Split(',');

            foreach (string al in split_al)
            {
                if (al.Equals("Hybrid"))
                {
                    algorithm = "Hybrid";
                }
                else
                    algorithm = "Tarantula";

            }

            int col_algorithm = ds.Tables["SourceCode"].Columns[algorithm].Ordinal;

            int row_last = ds.Tables["SourceCode"].Rows.Count;
            string value;

            for (int row = 1; row < row_last - 1; row++)
            {

                value = ds.Tables["SourceCode"].Rows[row][algorithm].ToString();

                if (!value.Equals(" ") && double.Parse(value) > threshold1)
                {
                    dgvSourceCode.Rows[row].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 255, 255, 0);
                }
                if (!value.Equals(" ") && double.Parse(value) > threshold2)
                {
                    dgvSourceCode.Rows[row].DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 255, 0, 0);
                }
            }

        }

        private void dgvMain_CellPainting(object sender, System.Windows.Forms.DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex == -1 && flag_loadTC)
            {

                e.PaintBackground(e.ClipBounds, false);

                System.Drawing.Point pt = e.CellBounds.Location;  // where you want the bitmap in the cell

                int nChkBoxWidth = 15;
                int nChkBoxHeight = 15;
                int offsetx = (e.CellBounds.Width - nChkBoxWidth) / 2;
                int offsety = (e.CellBounds.Height - nChkBoxHeight) / 2;

                pt.X += offsetx;
                pt.Y += offsety;

                //System.Windows.Forms.CheckBox cb = new System.Windows.Forms.CheckBox();
                cb_dgvMain.Size = new System.Drawing.Size(nChkBoxWidth, nChkBoxHeight);
                cb_dgvMain.Location = pt;
                cb_dgvMain.CheckedChanged += new EventHandler(dgvMainListCheckBox_CheckedChanged);

                ((DataGridView)sender).Controls.Add(cb_dgvMain);

                e.Handled = true;
                flag_loadTC = false;
            }
        }


        private void dgvMainListCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dgvMain.Rows)
            {
                r.Cells["Select"].Value = ((System.Windows.Forms.CheckBox)sender).Checked;
            }
        }

        private void dgvMain_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (dgvMain.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    //System.Windows.Forms.MessageBox.Show(this.dgvMain.CurrentCell.Value.ToString());
                    if ((bool)dgvMain.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == true)
                    {
                        cnt_selectedTC++;
                    }
                    else
                    {
                        cnt_selectedTC--;
                    }

                }
            }
            tbSelectedTC.Text = cnt_selectedTC.ToString();

        }

        private void dgvMain_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (this.dgvMain.IsCurrentCellDirty)
            {
                this.dgvMain.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void ContextMenuClickEventHandler(object sender, RoutedEventArgs e)
        {

        }

        private void dgvMain_Scroll(object sender, ScrollEventArgs e)
        {
            if (!flag_cb_dgvMain)
            {
                nScroll_dgvMain = e.OldValue;
                flag_cb_dgvMain = true;
            }

            if (e.NewValue == nScroll_dgvMain)
            {
                cb_dgvMain.Visible = true;
            }
            else
            {
                cb_dgvMain.Visible = false;
            }
        }

        private void tbSelectedTC_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbSelectedTC.Text.Equals("0"))
                cb_dgvMain.Checked = false;
            if (tbSelectedTC.Text.Equals(tbTotalTC.Text))
                cb_dgvMain.Checked = true;

        }

        private void dgvMain_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        public void SaveTestResult()
        {
            string strSourceCodeFilename = string.Empty;
            string strTestSuiteFilename = string.Empty;

            if (!string.IsNullOrEmpty(TestInfo.SourceCode))
                strSourceCodeFilename = System.IO.Path.GetFileNameWithoutExtension(TestInfo.SourceCode);

            if (!string.IsNullOrEmpty(TestInfo.TestSuite))
                strTestSuiteFilename = System.IO.Path.GetFileNameWithoutExtension(TestInfo.TestSuite);

            string strProgramName = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strProgramFilename);
            string strAnswerSheet = System.IO.Path.GetFileNameWithoutExtension(TestInfo.strAnswerSheet);


            string strTestResultFilename = string.Empty;
            if (chkDBFile.IsChecked == true)
            {
                strTestResultFilename = TestInfo.strResultPath + @"TestResult_" + strProgramName + "_" + TestInfo.strFaultyVersion + "_" + strAnswerSheet + ".xlsx";

            }
            else
            {
                strTestResultFilename = TestInfo.strResultPath + @"TestResult_" + strSourceCodeFilename + "_" + strTestSuiteFilename + "_" + strProgramName + "_" + TestInfo.strFaultyVersion + "_" + strAnswerSheet + ".xlsx";
            }

            if (File.Exists(strTestResultFilename))
                File.Delete(strTestResultFilename);

            if (dsTestCase.Tables.Count == 0) //database case
            {
                //if (chkAnswerSheet.IsChecked != true)
                //{
                //    WriteToExcelSheet(strTestResultFilename, null, dsSourceCode.Tables["SourceCode"], dsDistinct.Tables[0], null);
                //}
                //else

                //WriteToExcelSheet(strTestResultFilename, null, dsSourceCode.Tables["SourceCode"], dsFSourceCode.Tables["FSourceCode"], dsPSourceCode.Tables["PSourceCode"], dsDistinct.Tables[0], dsResult.Tables[0]);
                WriteToExcelSheet2(strTestResultFilename, null, dsSourceCode.Tables["SourceCode"], dsResult.Tables[0]);
                //WriteToExcelSheet3(strTestResultFilename, null, dsSourceCode.Tables["SourceCode"], dsFSourceCode.Tables["FSourceCode"], dsPSourceCode.Tables["PSourceCode"], dsResult.Tables[0]);

            }
            else
            {
                //if (chkAnswerSheet.IsChecked != true)
                //    WriteToExcelSheet(strTestResultFilename, dsTestCase.Tables[0], dsSourceCode.Tables["SourceCode"], dsDistinct.Tables[0], null);
                //else
                //WriteToExcelSheet(strTestResultFilename, dsTestCase.Tables[0], dsSourceCode.Tables["SourceCode"], dsFSourceCode.Tables["FSourceCode"], dsPSourceCode.Tables["PSourceCode"], dsDistinct.Tables[0], dsResult.Tables[0]);
                WriteToExcelSheet2(strTestResultFilename, dsTestCase.Tables[0], dsSourceCode.Tables["SourceCode"], dsResult.Tables[0]);
                //WriteToExcelSheet3(strTestResultFilename, null, dsSourceCode.Tables["SourceCode"], dsFSourceCode.Tables["FSourceCode"], dsPSourceCode.Tables["PSourceCode"], dsResult.Tables[0]);

            }

            // FInal Report
            string strFinalReportPath = Environment.CurrentDirectory + @"\Result\FinalReport.xlsx";
            if (!File.Exists(strFinalReportPath)) // no first file -> create one
            {
                WriteFinalReport(strFinalReportPath, dsResult.Tables[0]);
            }
            else
            {
                DataSet temp_ds = NativeMethod.GetDataFromExcel(strFinalReportPath);
                dsResult.Tables[0].Merge(temp_ds.Tables[0]);
                //dsResult.Merge(temp_ds);
                WriteFinalReport(strFinalReportPath, dsResult.Tables[0]);
            }



        }

        private void btnSaveTestResult_Click(object sender, RoutedEventArgs e)
        {
            //ExcelLibrary.DataSetHelper.CreateWorkbook(TestInfo.strResultPath + "TestResult.xls", dsTestCase);
            //            ExcelLibrary.DataSetHelper.CreateWorkbook(TestInfo.strResultPath + "TestResult.xls", dsSourceCode);
            SaveTestResult();


        }

        private void chkShowTC_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;
            if (cb.Name == chkShowTC.Name)
            {

                int col_TC_ID = dsSourceCode.Tables["SourceCode"].Columns["SourceCode"].Ordinal + 1;

                for (int i = col_TC_ID; i < dgvSourceCode.ColumnCount; i++)
                {
                    if (dgvSourceCode.Columns[i].Name.Contains("TC_"))
                    {

                        dgvSourceCode.Columns[i].Visible = false;

                    }
                }
            }
        }

        private void chkShowTC_Unchecked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;
            if (cb.Name == chkShowTC.Name)
            {

                int col_TC_ID = dsSourceCode.Tables["SourceCode"].Columns["SourceCode"].Ordinal + 1;

                for (int i = col_TC_ID; i < dgvSourceCode.ColumnCount; i++)
                {
                    if (dgvSourceCode.Columns[i].Name.Contains("TC_"))
                    {

                        dgvSourceCode.Columns[i].Visible = true;

                    }
                }
            }

        }

        public void WriteToExcelSheet(string path, DataTable dtSourceCode, DataTable dtFSourceCode, DataTable dtPSourceCode, DataTable dtDistinct, DataTable dtResult)
        {
            try
            {
                Directory.CreateDirectory(TestInfo.strResultPath);
            }
            catch (Exception e)
            {
            }
            FileInfo workBook = null;
            try
            {
                //create FileInfo object  to read you ExcelWorkbook
                workBook = new FileInfo(path);
                using (ExcelPackage xlPackage = new ExcelPackage(workBook))
                {
                    ExcelWorksheet wsTestCase = xlPackage.Workbook.Worksheets.Add("SourceCode");
                    wsTestCase.Cells["A1"].LoadFromDataTable(dtSourceCode, true);

                    // ExcelWorksheet wsFTestCase = xlPackage.Workbook.Worksheets.Add("FSourceCode");
                    // wsFTestCase.Cells["A1"].LoadFromDataTable(dtSourceCode, true);

                    //ExcelWorksheet wsPTestCase = xlPackage.Workbook.Worksheets.Add("PSourceCode");
                    //wsPTestCase.Cells["A1"].LoadFromDataTable(dtSourceCode, true);

                    //ExcelWorksheet wsSourceCode = xlPackage.Workbook.Worksheets.Add("Distinct");
                    //wsSourceCode.Cells["A1"].LoadFromDataTable(dtDistinct, true);

                    if (chkAnswerSheet.IsChecked == true)
                    {
                        ExcelWorksheet wsSourceCode2 = xlPackage.Workbook.Worksheets.Add("Result");
                        wsSourceCode2.Cells["A1"].LoadFromDataTable(dtResult, true);
                    }

                    xlPackage.Save();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //set workbook object to null
                if (workBook != null)
                    workBook = null;
            }
        }

        public void WriteToExcelSheet(string path, DataTable dtTestCase, DataTable dtSourceCode, DataTable dtFSourceCode, DataTable dtPSourceCode, DataTable dtDistinct, DataTable dtResult)
        {
            try
            {
                Directory.CreateDirectory(TestInfo.strResultPath);
            }
            catch (Exception e)
            {
            }

            FileInfo workBook = null;
            try
            {
                //create FileInfo object  to read you ExcelWorkbook
                workBook = new FileInfo(path);
                using (ExcelPackage xlPackage = new ExcelPackage(workBook))
                {
                    if (dtTestCase != null)
                    {
                        ExcelWorksheet wsTestCase = xlPackage.Workbook.Worksheets.Add("TestCase");
                        wsTestCase.Cells["A1"].LoadFromDataTable(dtTestCase, true);
                    }

                    ExcelWorksheet wsSourceCode = xlPackage.Workbook.Worksheets.Add("SourceCode");
                    wsSourceCode.Cells["A1"].LoadFromDataTable(dtSourceCode, true);

                    ExcelWorksheet wsFSourceCode = xlPackage.Workbook.Worksheets.Add("FSourceCode");
                    wsFSourceCode.Cells["A1"].LoadFromDataTable(dtFSourceCode, true);

                    ExcelWorksheet wsPSourceCode = xlPackage.Workbook.Worksheets.Add("PSourceCode");
                    wsPSourceCode.Cells["A1"].LoadFromDataTable(dtPSourceCode, true);

                    //   ExcelWorksheet wsSourceCode2 = xlPackage.Workbook.Worksheets.Add("Distinct");
                    //    wsSourceCode2.Cells["A1"].LoadFromDataTable(dtDistinct, true);

                    ExcelWorksheet wsSourceCode3 = xlPackage.Workbook.Worksheets.Add("Result");
                    wsSourceCode3.Cells["A1"].LoadFromDataTable(dtResult, true);

                    xlPackage.Save();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //set workbook object to null
                if (workBook != null)
                    workBook = null;
            }
        }

        public void WriteToExcelSheet2(string path, DataTable dtTestCase, DataTable dtSourceCode, DataTable dtResult)
        {
            try
            {
                Directory.CreateDirectory(TestInfo.strResultPath);
            }
            catch (Exception e)
            {
            }

            FileInfo workBook = null;
            try
            {
                //create FileInfo object  to read you ExcelWorkbook
                workBook = new FileInfo(path);
                using (ExcelPackage xlPackage = new ExcelPackage(workBook))
                {
                    if (dtTestCase != null)
                    {
                        ExcelWorksheet wsTestCase = xlPackage.Workbook.Worksheets.Add("TestCase");
                        wsTestCase.Cells["A1"].LoadFromDataTable(dtTestCase, true);
                    }

                    //ExcelWorksheet wsSourceCode = xlPackage.Workbook.Worksheets.Add("SourceCode");
                    //wsSourceCode.Cells["A1"].LoadFromDataTable(dtSourceCode, true);
                    //ExcelWorksheet wsFSourceCode = xlPackage.Workbook.Worksheets.Add("FSourceCode");
                    //wsFSourceCode.Cells["A1"].LoadFromDataTable(dtFSourceCode, true);

                    //ExcelWorksheet wsPSourceCode = xlPackage.Workbook.Worksheets.Add("PSourceCode");
                    //wsPSourceCode.Cells["A1"].LoadFromDataTable(dtPSourceCode, true);
                    //   ExcelWorksheet wsSourceCode2 = xlPackage.Workbook.Worksheets.Add("Distinct");
                    //    wsSourceCode2.Cells["A1"].LoadFromDataTable(dtDistinct, true);

                    ExcelWorksheet wsSourceCode3 = xlPackage.Workbook.Worksheets.Add("Result");
                    wsSourceCode3.Cells["A1"].LoadFromDataTable(dtResult, true);

                    xlPackage.Save();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());

            }
            finally
            {
                //set workbook object to null
                if (workBook != null)
                    workBook = null;
            }
        }

        public void WriteToExcelSheet3(string path, DataTable dtTestCase, DataTable dtSourceCode, DataTable dtFSourceCode, DataTable dtPSourceCode, DataTable dtResult)
        {
            try
            {
                Directory.CreateDirectory(TestInfo.strResultPath);
            }
            catch (Exception e)
            {
            }

            FileInfo workBook = null;
            try
            {
                //create FileInfo object  to read you ExcelWorkbook
                workBook = new FileInfo(path);
                using (ExcelPackage xlPackage = new ExcelPackage(workBook))
                {
                    if (dtTestCase != null)
                    {
                        ExcelWorksheet wsTestCase = xlPackage.Workbook.Worksheets.Add("TestCase");
                        wsTestCase.Cells["A1"].LoadFromDataTable(dtTestCase, true);
                    }

                    ExcelWorksheet wsSourceCode = xlPackage.Workbook.Worksheets.Add("SourceCode");
                    wsSourceCode.Cells["A1"].LoadFromDataTable(dtSourceCode, true);

                    ExcelWorksheet wsFSourceCode = xlPackage.Workbook.Worksheets.Add("FSourceCode");
                    wsFSourceCode.Cells["A1"].LoadFromDataTable(dtFSourceCode, true);

                    ExcelWorksheet wsPSourceCode = xlPackage.Workbook.Worksheets.Add("PSourceCode");
                    wsPSourceCode.Cells["A1"].LoadFromDataTable(dtPSourceCode, true);


                    //   ExcelWorksheet wsSourceCode2 = xlPackage.Workbook.Worksheets.Add("Distinct");
                    //    wsSourceCode2.Cells["A1"].LoadFromDataTable(dtDistinct, true);

                    ExcelWorksheet wsSourceCode3 = xlPackage.Workbook.Worksheets.Add("Result");
                    wsSourceCode3.Cells["A1"].LoadFromDataTable(dtResult, true);

                    xlPackage.Save();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());

            }
            finally
            {
                //set workbook object to null
                if (workBook != null)
                    workBook = null;
            }
        }

        public void WriteFinalReport(string path, DataTable ds)
        {

            if (File.Exists(path))
                File.Delete(path);

            FileInfo workBook = null;
            try
            {
                //create FileInfo object  to read you ExcelWorkbook
                workBook = new FileInfo(path);
                using (ExcelPackage xlPackage = new ExcelPackage(workBook))
                {

                    ExcelWorksheet wsSourceCode = xlPackage.Workbook.Worksheets.Add("FinalReport");
                    wsSourceCode.Cells["A1"].LoadFromDataTable(ds, true);

                    xlPackage.Save();
                }
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                //set workbook object to null
                if (workBook != null)
                    workBook = null;
            }
        }

        public void SelectAllTC()
        {
            if (btnSelectAll.Content.Equals("Select All"))
            {
                btnSelectAll.Content = "Unselect All";

                for (int i = 0; i < dgvMain.Rows.Count; i++)
                {
                    dgvMain.Rows[i].Cells["Select"].Value = true;
                }
            }
            else
            {
                btnSelectAll.Content = "Select All";
                for (int i = 0; i < dgvMain.Rows.Count; i++)
                {
                    dgvMain.Rows[i].Cells["Select"].Value = false;
                }

            }
        }
        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            SelectAllTC();
        }

        private void btnDBFile_Click(object sender, RoutedEventArgs e)
        {
            tbDBFile.Text = NativeMethod.getFileFullName("xls;xlsx");
            chkDBFile.IsChecked = true;
            IniFile.SetIniValue("Database", "ProgramFilename", tbDBFile.Text, Environment.CurrentDirectory + @"\Setting.ini");
            TestInfo.strProgramFilename = System.IO.Path.GetFileNameWithoutExtension(tbDBFile.Text);

            //System.IO.Path.GetExtension(tbInstrumentedCode.Text);
        }

        private void chkDBFile_Checked(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.CheckBox cb = sender as System.Windows.Controls.CheckBox;
            if (cb.Name == chkDBFile.Name)
            {

            }

        }

        private void chkDBFile_Unchecked(object sender, RoutedEventArgs e)
        {
            tbDBFile.Text = string.Empty;
            IniFile.SetIniValue("Database", "ProgramFilename", tbDBFile.Text, Environment.CurrentDirectory + @"\Setting.ini");
        }

        private void chkAnswerSheet_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void chkAnswerSheet_Unchecked(object sender, RoutedEventArgs e)
        {
            tbAnswerSheet.Text = string.Empty;
            IniFile.SetIniValue("Database", "AnswerSheet", tbAnswerSheet.Text, Environment.CurrentDirectory + @"\Setting.ini");
        }

        private void btnAnswerSheet_Click(object sender, RoutedEventArgs e)
        {
            tbAnswerSheet.Text = NativeMethod.getFileFullName("xls;xlsx");
            chkAnswerSheet.IsChecked = true;
            IniFile.SetIniValue("Database", "AnswerSheet", tbAnswerSheet.Text, Environment.CurrentDirectory + @"\Setting.ini");
            TestInfo.strFaultyVersion = System.IO.Path.GetFileNameWithoutExtension(tbAnswerSheet.Text);
        }

        private void tbFaultyVer_TextChanged(object sender, TextChangedEventArgs e)
        {
            TestInfo.strFaultyVersion = tbFaultyVer.Text;
            IniFile.SetIniValue("Database", "FaultyVersion", tbFaultyVer.Text, Environment.CurrentDirectory + @"\Setting.ini");
        }




    }
}

