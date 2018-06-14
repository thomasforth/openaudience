using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OpenAudienceCalculations
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. LOAD ALL THE FILES
            string baseDirectory = @"../../../../../InputFiles/";

            // Output area datasets from Nomis and put them in a keyed dictionary
            // QS103EWDATA = Age by single year.
            TextReader ageTextReader = File.OpenText(baseDirectory + "QS103EWDATA.CSV");
            CsvReader ageCSVReader = new CsvReader(ageTextReader);
            List<Age> Ages = ageCSVReader.GetRecords<Age>().ToList();


            // In the age dataset ID = K04000001 is for the whole of England and Wales
            Dictionary<string, Age> AgesDictionary = new Dictionary<string, Age>();
            foreach (Age age in Ages)
            {
                AgesDictionary.Add(age.GeographyCode, age);
            }

            // QS201EWDATA = Ethnic group.
            TextReader ethnicTextReader = File.OpenText(baseDirectory + "QS201EWDATA.CSV");
            CsvReader ethnicCSVReader = new CsvReader(ethnicTextReader);
            List<EthnicGroup> EthnicGroups = ethnicCSVReader.GetRecords<EthnicGroup>().ToList();

            Dictionary<string, EthnicGroup> EthnicGroupsDictionary = new Dictionary<string, EthnicGroup>();
            foreach (EthnicGroup ethnicgroup in EthnicGroups)
            {
                EthnicGroupsDictionary.Add(ethnicgroup.GeographyCode, ethnicgroup);
            }

            // QS611EWDATA = Approximated Social Grade.
            TextReader classTextReader = File.OpenText(baseDirectory + "QS611EWDATA.CSV");
            CsvReader classCSVReader = new CsvReader(classTextReader);
            List<SocialGrade> SocialGrades = classCSVReader.GetRecords<SocialGrade>().ToList();

            Dictionary<string, SocialGrade> SocialGradesDictionary = new Dictionary<string, SocialGrade>();
            foreach (SocialGrade socialgrade in SocialGrades)
            {
                SocialGradesDictionary.Add(socialgrade.GeographyCode, socialgrade);
            }

            // KS501EWDATA.CSV = skills data
            TextReader skillsTextReader = File.OpenText(baseDirectory + "KS501EWDATA.CSV");
            CsvReader skillsCSVReader = new CsvReader(skillsTextReader);
            List<Skill> Skills = skillsCSVReader.GetRecords<Skill>().ToList();

            Dictionary<string, Skill> SkillsDictionary = new Dictionary<string, Skill>();
            foreach (Skill skill in Skills)
            {
                SkillsDictionary.Add(skill.GeographyCode, skill);
            }

            // OA_Events.csv = Eventbrite Tech Events by output area
            TextReader eventsTextReader = File.OpenText(baseDirectory + "OA_Events.csv");
            CsvReader eventsCSVReader = new CsvReader(eventsTextReader);
            List<Event> Events = eventsCSVReader.GetRecords<Event>().ToList();

            Dictionary<string, Event> EventsDictionary = new Dictionary<string, Event>();
            foreach (Event _event in Events)
            {
                EventsDictionary.Add(_event.OutputArea, _event);
            }

            // Output area datasets from ONS
            // 2011 OAC Clusters and Names csv v2.csv = Pen portrait by OA.
            TextReader penTextReader = File.OpenText(baseDirectory + "2011 OAC Clusters and Names csv v2.csv");
            CsvReader penCSVReader = new CsvReader(penTextReader);
            penCSVReader.Configuration.RegisterClassMap<PenPortraitMap>();
            List<PenPortrait> PenPortraits = penCSVReader.GetRecords<PenPortrait>().ToList();

            Dictionary<string, PenPortrait> PenPortraitDictionary = new Dictionary<string, PenPortrait>();
            foreach (PenPortrait penportrait in PenPortraits)
            {
                PenPortraitDictionary.Add(penportrait.OutputAreaCode, penportrait);
            }

            // Link table
            // Output_Area_to_Local_Authority_District_to_Lower_Layer_Super_Output_Area_to_Middle_Layer_Super_Output_Area_to_Local_Enterprise_Partnership_April_2017_Lookup_in_England_V2.csv
            TextReader linkTextReader = File.OpenText(baseDirectory + "Output_Area_to_Ward_to_Local_Authority_District_December_2017_Lookup_in_England_and_Wales.csv");
            CsvReader linkCSVReader = new CsvReader(linkTextReader);
            List<OALink> OALinks = linkCSVReader.GetRecords<OALink>().ToList();

            Dictionary<string, OALink> OAToWardDictionary = new Dictionary<string, OALink>();
            foreach (OALink oalink in OALinks)
            {
                OAToWardDictionary.Add(oalink.OA11CD, oalink);
            }

            // Another Link Table            
            TextReader msoalinkTextReader = File.OpenText(baseDirectory + "Output_Area_to_Local_Authority_District_to_Lower_Layer_Super_Output_Area_to_Middle_Layer_Super_Output_Area_to_Local_Enterprise_Partnership_April_2017_Lookup_in_England_V2.csv");
            CsvReader msoalinkCSVReader = new CsvReader(msoalinkTextReader);
            List<OAMSOALink> OAMSOALinks = msoalinkCSVReader.GetRecords<OAMSOALink>().ToList();

            Dictionary<string, OAMSOALink> OAToMSOADictionary = new Dictionary<string, OAMSOALink>();
            foreach (OAMSOALink oamsoalink in OAMSOALinks)
            {
                OAToMSOADictionary.Add(oamsoalink.OA11CD, oamsoalink);
            }

            // Yet another link table
            TextReader NUTSlinkTextReader = File.OpenText(baseDirectory + "OA11_LAU2_LAU1_NUTS3_NUTS2_NUTS1_EW_LU.csv");
            CsvReader NUTSlinkCSVReader = new CsvReader(NUTSlinkTextReader);
            List<OANUTSLink> OANUTSLinks = NUTSlinkCSVReader.GetRecords<OANUTSLink>().ToList();

            Dictionary<string, OANUTSLink> OANUTSLinkDictionary = new Dictionary<string, OANUTSLink>();
            foreach (OANUTSLink oasnutslink in OANUTSLinks)
            {
                OANUTSLinkDictionary.Add(oasnutslink.OA11CD, oasnutslink);
            }


            // MSOA income data table
            // 1netannualincomeahc.csv = Income after housing costs by OA.
            // THIS FILE CONTAINS LINES AT THE START THAT NEED DELETING, AND IS ENCODED AS ANSI AND NEEDS CONVERTING TO UTF-8. BOTH CAN BE DONE IN NOTEPAD++
            TextReader incomeTextReader = File.OpenText(baseDirectory + "1netannualincomeahc.csv");
            CsvReader incomeCSVReader = new CsvReader(incomeTextReader);
            incomeCSVReader.Configuration.RegisterClassMap<IncomeMap>();
            List<IncomeAHC> Incomes = incomeCSVReader.GetRecords<IncomeAHC>().OrderBy(x => x.CleanNetannualincomeafterhousingcosts).ToList();

            Dictionary<string, IncomeAHC> MSOAToIncomeDictionary = new Dictionary<string, IncomeAHC>();

            // clean up Incomes and add them to a dictionary
            foreach (IncomeAHC income in Incomes)
            {
                double cleanNAIAHC = 0;
                Double.TryParse(income.Netannualincomeafterhousingcosts.Replace(" ", "").Replace(",", ""), out cleanNAIAHC);
                income.CleanNetannualincomeafterhousingcosts = cleanNAIAHC;
                MSOAToIncomeDictionary.Add(income.MSOAcode, income);
            }

            // 2. DO ALL THE CALCULATIONS

            // get unique Output Areas
            List<string> UniqueOutputAreas = Ages.Select(x => x.GeographyCode).Distinct().ToList();

            List<IncomeAHC> IncomesWithoutZeros = Incomes.Where(x => x.CleanNetannualincomeafterhousingcosts != 0).OrderBy(x => x.CleanNetannualincomeafterhousingcosts).ToList();

            // define cut-offs for income (at 1/3 and 2/3)
            double lowerthirdcutoff = IncomesWithoutZeros[IncomesWithoutZeros.Count() / 3].CleanNetannualincomeafterhousingcosts;
            double upperthirdcutoff = IncomesWithoutZeros[(2 * IncomesWithoutZeros.Count()) / 3].CleanNetannualincomeafterhousingcosts;

            // calculate total banded age percentages
            BandedAgePercent WholeCountryBandedAges = BandAgesPercent(AgesDictionary["K04000001"]);

            List<OAInfo> OutputAreaDescriptions = new List<OAInfo>();
            foreach (string UniqueOA in UniqueOutputAreas)
            {
                OAInfo outputarea = new OAInfo();
                outputarea.OutputArea = UniqueOA;

                Skill skill;
                SkillsDictionary.TryGetValue(UniqueOA, out skill);
                if (skill != null)
                {
                    outputarea.GraduateCount = skill.KS501EW0007;
                    outputarea.StudentCount = skill.KS501EW0011 + skill.KS501EW0012 + skill.KS501EW0013;
                }

                Event _event;
                EventsDictionary.TryGetValue(UniqueOA, out _event);
                if (_event != null)
                {
                    outputarea.TechEventCount = _event.EventCount;
                }


                OAMSOALink MSOA;
                OAToMSOADictionary.TryGetValue(UniqueOA, out MSOA);
                if (MSOA != null)
                {
                    outputarea.MSOA = MSOA.MSOA11CD;
                    outputarea.Localauthorityname = MSOA.LAD16NM;

                    IncomeAHC income;
                    MSOAToIncomeDictionary.TryGetValue(MSOA.MSOA11CD, out income);
                    if (income != null)
                    {
                        outputarea.MedianIncomeAHC = income.CleanNetannualincomeafterhousingcosts;
                        outputarea.IncomeGroup = IncomeToIncomeClassification(income, lowerthirdcutoff, upperthirdcutoff);
                    }
                }

                OALink Ward;
                OAToWardDictionary.TryGetValue(UniqueOA, out Ward);
                if (Ward != null)
                {
                    outputarea.WardCode = Ward.WD17CD;
                    outputarea.WardName = Ward.WD17NM;
                }

                OANUTSLink oANUTSLink;
                OANUTSLinkDictionary.TryGetValue(UniqueOA, out oANUTSLink);
                if (oANUTSLink != null)
                {
                    outputarea.NUTS2 = oANUTSLink.NUTS206CD;
                }

                PenPortrait penPortrait;
                PenPortraitDictionary.TryGetValue(UniqueOA, out penPortrait);
                if (penPortrait != null)
                {
                    outputarea.PenPortrait = penPortrait.SupergroupName;
                }

                EthnicGroup ethnicGroup;
                EthnicGroupsDictionary.TryGetValue(UniqueOA, out ethnicGroup);
                if (ethnicGroup != null)
                {
                    outputarea.PercentWhite = ethnicGroup.QS201EW0002 / ethnicGroup.QS201EW0001;
                }

                Age age;
                AgesDictionary.TryGetValue(UniqueOA, out age);
                if (age != null)
                {
                    outputarea.Population = age.QS103EW0001;
                    outputarea.BandedAgeCounts = BandAgesCount(age);
                    outputarea.BandedAgePercentages = BandAgesPercent(age);
                    outputarea.AgeProfile = CalculateAgeProfile(BandAgesPercent(age), WholeCountryBandedAges);
                }


                List<string> SocialGradeName = new List<string> { "AB", "C1", "C2", "DE" };
                SocialGrade socialGrade;
                SocialGradesDictionary.TryGetValue(UniqueOA, out socialGrade);
                if (socialGrade != null)
                {
                    List<double> socialGradesArray = new List<double> { socialGrade.QS611EW0002, socialGrade.QS611EW0003, socialGrade.QS611EW0004, socialGrade.QS611EW0005 };
                    outputarea.LargestSocialGrade = SocialGradeName.ElementAt(socialGradesArray.IndexOf(socialGradesArray.Max()));

                    outputarea.SocialGradeCounts = SocialGradesToCounts(socialGrade);
                    outputarea.SocialGradePercentages = SocialGradesToPercentages(socialGrade);
                }

                outputarea.CombinedProfile = outputarea.IncomeGroup + " " + outputarea.AgeProfile;

                OutputAreaDescriptions.Add(outputarea);
            }

            // Calculate baselines
            List<OAInfo> nonnullinfos = OutputAreaDescriptions.Where(x => x.MSOA != null).ToList();

            OAInfo UKBaseline = new OAInfo();
            Baseline UKbaseline = new Baseline()
            {
                IncomeCounts = new IncomeCount()
                {
                    LowIncomeCount = 0,
                    MiddleIncomeCount = 0,
                    HighIncomeCount = 0
                },
                PenPortraitCounts = new PenPortraitCount()
                {
                    ConstrainedCityDwellers = 0,
                    Cosmopolitans = 0,
                    EthnicityCentral = 0,
                    HardPressedLiving = 0,
                    MulticulturalMetropolitans = 0,
                    RuralResidents = 0,
                    Suburbanites = 0,
                    Urbanites = 0
                },
                BandedAges = new BandedAgeCount()
                {
                    All = 0,
                    NineteenToThirty = 0,
                    SixtyPlus = 0,
                    ThirteenToEighteen = 0,
                    ThirtyOneToFiftyNine = 0,
                    ZeroToTwelve = 0
                },
                SocialGrades = new SocialGradeCount()
                {
                    ABCount = 0,
                    AllCount = 0,
                    C1Count = 0,
                    C2Count = 0,
                    DECount = 0
                },
                AgeProfileCounts = new AgeProfileCount()
                {
                    OlderFamily = 0,
                    OlderPeople = 0,
                    StudentsYoungPros = 0,
                    YoungFamily = 0
                },
                CombinedProfileCounts = new CombinedProfileCount()
                {
                    PoorerOlderFamily = 0,
                    PoorerOlderPeople = 0,
                    WealthyYoungFamily = 0,
                    PoorerYoungFamily = 0,
                    StudentsYoungPros = 0,
                    WealthyOlderFamily = 0,
                    WealthyOlderPeople = 0
                }
            
            };
            double WhiteCount = 0;
            double PopulationCount = 0;

            foreach (OAInfo oainfo in nonnullinfos)
            {
                UKbaseline.Place = "EnglandAndWales";
                UKbaseline.MeanMedianIncomeAHC += oainfo.MedianIncomeAHC;
                WhiteCount += oainfo.PercentWhite * oainfo.Population;
                PopulationCount += oainfo.Population;

                UKbaseline.BandedAges.All += oainfo.BandedAgeCounts.All;
                UKbaseline.BandedAges.ZeroToTwelve += oainfo.BandedAgeCounts.ZeroToTwelve;
                UKbaseline.BandedAges.ThirteenToEighteen += oainfo.BandedAgeCounts.ThirteenToEighteen;
                UKbaseline.BandedAges.NineteenToThirty += oainfo.BandedAgeCounts.NineteenToThirty;
                UKbaseline.BandedAges.ThirtyOneToFiftyNine += oainfo.BandedAgeCounts.ThirtyOneToFiftyNine;
                UKbaseline.BandedAges.SixtyPlus += oainfo.BandedAgeCounts.SixtyPlus;

                UKbaseline.SocialGrades.AllCount += oainfo.SocialGradeCounts.AllCount;
                UKbaseline.SocialGrades.ABCount += oainfo.SocialGradeCounts.ABCount;
                UKbaseline.SocialGrades.C1Count += oainfo.SocialGradeCounts.C1Count;
                UKbaseline.SocialGrades.C2Count += oainfo.SocialGradeCounts.C2Count;
                UKbaseline.SocialGrades.DECount += oainfo.SocialGradeCounts.DECount;

                switch (oainfo.IncomeGroup)
                {
                    case "Low income":
                        UKbaseline.IncomeCounts.LowIncomeCount = UKbaseline.IncomeCounts.LowIncomeCount + 1;
                        break;
                    case "Middle income":
                        UKbaseline.IncomeCounts.MiddleIncomeCount = UKbaseline.IncomeCounts.MiddleIncomeCount + 1;
                        break;
                    case "High income":
                        UKbaseline.IncomeCounts.HighIncomeCount = UKbaseline.IncomeCounts.HighIncomeCount + 1;
                        break;
                }

                switch (oainfo.PenPortrait)
                {
                    case "Constrained City Dwellers":
                        UKbaseline.PenPortraitCounts.ConstrainedCityDwellers = UKbaseline.PenPortraitCounts.ConstrainedCityDwellers + 1;
                        break;
                    case "Cosmopolitans":
                        UKbaseline.PenPortraitCounts.Cosmopolitans = UKbaseline.PenPortraitCounts.Cosmopolitans + 1;
                        break;
                    case "Ethnicity Central":
                        UKbaseline.PenPortraitCounts.EthnicityCentral = UKbaseline.PenPortraitCounts.EthnicityCentral + 1;
                        break;
                    case "Hard-Pressed Living":
                        UKbaseline.PenPortraitCounts.HardPressedLiving = UKbaseline.PenPortraitCounts.HardPressedLiving + 1;
                        break;
                    case "Multicultural Metropolitans":
                        UKbaseline.PenPortraitCounts.MulticulturalMetropolitans = UKbaseline.PenPortraitCounts.MulticulturalMetropolitans + 1;
                        break;
                    case "Rural Residents":
                        UKbaseline.PenPortraitCounts.RuralResidents = UKbaseline.PenPortraitCounts.RuralResidents + 1;
                        break;
                    case "Suburbanites":
                        UKbaseline.PenPortraitCounts.Suburbanites = UKbaseline.PenPortraitCounts.Suburbanites + 1;
                        break;
                    case "Urbanites":
                        UKbaseline.PenPortraitCounts.Urbanites = UKbaseline.PenPortraitCounts.Urbanites + 1;
                        break;
                }

                switch (oainfo.AgeProfile)
                {
                    case "Older family":
                        UKbaseline.AgeProfileCounts.OlderFamily = UKbaseline.AgeProfileCounts.OlderFamily + 1;
                        break;
                    case "Older people":
                        UKbaseline.AgeProfileCounts.OlderPeople = UKbaseline.AgeProfileCounts.OlderPeople + 1;
                        break;
                    case "Young adults":
                        UKbaseline.AgeProfileCounts.StudentsYoungPros = UKbaseline.AgeProfileCounts.StudentsYoungPros + 1;
                        break;
                    case "Young family":
                        UKbaseline.AgeProfileCounts.YoungFamily = UKbaseline.AgeProfileCounts.YoungFamily + 1;
                        break;
                }

                switch (oainfo.CombinedProfile)
                {
                    case "Low income Young adults":
                        UKbaseline.CombinedProfileCounts.StudentsYoungPros = UKbaseline.CombinedProfileCounts.StudentsYoungPros + 1;
                        break;
                    case "Middle income Young adults":
                        UKbaseline.CombinedProfileCounts.StudentsYoungPros = UKbaseline.CombinedProfileCounts.StudentsYoungPros + 1;
                        break;
                    case "High income Young adults":
                        UKbaseline.CombinedProfileCounts.StudentsYoungPros = UKbaseline.CombinedProfileCounts.StudentsYoungPros + 1;
                        break;
                    case "Low income Young family":
                        UKbaseline.CombinedProfileCounts.PoorerYoungFamily = UKbaseline.CombinedProfileCounts.PoorerYoungFamily + 1;
                        break;
                    case "Middle income Young family":
                        UKbaseline.CombinedProfileCounts.WealthyYoungFamily = UKbaseline.CombinedProfileCounts.WealthyYoungFamily + 1;
                        break;
                    case "High income Young family":
                        UKbaseline.CombinedProfileCounts.WealthyYoungFamily = UKbaseline.CombinedProfileCounts.WealthyYoungFamily + 1;
                        break;
                    case "Low income Older family":
                        UKbaseline.CombinedProfileCounts.PoorerOlderFamily = UKbaseline.CombinedProfileCounts.PoorerOlderFamily + 1;
                        break;
                    case "Middle income Older family":
                        UKbaseline.CombinedProfileCounts.WealthyOlderFamily = UKbaseline.CombinedProfileCounts.WealthyOlderFamily + 1;
                        break;
                     case "High income Older family":
                        UKbaseline.CombinedProfileCounts.WealthyOlderFamily = UKbaseline.CombinedProfileCounts.WealthyOlderFamily + 1;
                        break;
                     case "Low income Older people":
                        UKbaseline.CombinedProfileCounts.PoorerOlderPeople = UKbaseline.CombinedProfileCounts.PoorerOlderPeople + 1;
                        break;
                     case "Middle income Older people":
                        UKbaseline.CombinedProfileCounts.WealthyOlderPeople = UKbaseline.CombinedProfileCounts.WealthyOlderPeople + 1;
                        break;
                     case "High income Older people":
                        UKbaseline.CombinedProfileCounts.WealthyOlderPeople = UKbaseline.CombinedProfileCounts.WealthyOlderPeople + 1;
                        break;
                }
            }
            UKbaseline.MeanMedianIncomeAHC = UKbaseline.MeanMedianIncomeAHC / nonnullinfos.Count();
            UKbaseline.PercentWhite = WhiteCount / PopulationCount;
            UKbaseline.Population = PopulationCount;

            List<Baseline> Baselines = new List<Baseline>();
            Baselines.Add(UKbaseline);

            // 3. OUTPUTS
            // WRITE THE OUTPUT AREA INFOS TABLE
            TextWriter _textWriter = File.CreateText(baseDirectory + "OutputAreaDescriptions.csv");
            CsvWriter _csvwriter = new CsvWriter(_textWriter);
            _csvwriter.WriteRecords(OutputAreaDescriptions);
            _textWriter.Dispose();
            _csvwriter.Dispose();


            // WRITE THE BASELINES TABLE
            TextWriter baselineTextWriter = File.CreateText(baseDirectory + "Baselines.csv");
            CsvWriter baselineCSVwriter = new CsvWriter(baselineTextWriter);
            baselineCSVwriter.WriteRecords(Baselines);
            baselineTextWriter.Dispose();
            baselineCSVwriter.Dispose();
        }

        static string CalculateAgeProfile(BandedAgePercent age, BandedAgePercent WholeCountryPercentage)
        {
            string AgeProfile = "";

            // calculate enirchments
            double youngFamilyEnrichment = age.ZeroToTwelvePercent / WholeCountryPercentage.ZeroToTwelvePercent;
            double olderFamilyEnrichment = age.ThirteenToEighteenPercent / WholeCountryPercentage.ThirteenToEighteenPercent;
            double youngProsEnrichment = age.NineteenToThirtyPercent / WholeCountryPercentage.NineteenToThirtyPercent;
            // age.ThirtyOneToFiftyNinePercent / WholeCountryPercentage.ThirtyOneToFiftyNinePercent;
            double olderPeopleEnrichment = 0.8 * age.SixtyPlusPercent / WholeCountryPercentage.SixtyPlusPercent;

            //SocialGradeName.ElementAt(socialGradesArray.IndexOf(socialGradesArray.Max()));

            List<double> Enrichments = new List<double> { youngFamilyEnrichment, olderFamilyEnrichment, youngProsEnrichment, olderPeopleEnrichment };
            int index = Enrichments.IndexOf(Enrichments.Max());

            List<string> EnrichmentNames = new List<string> { "Young family", "Older family", "Young adults", "Older people" };
            AgeProfile = EnrichmentNames[index];

            return AgeProfile;
        }

        static BandedAgeCount BandAgesCount(Age age)
        {
            BandedAgeCount bandedAgeCount = new BandedAgeCount();

            bandedAgeCount.All = age.QS103EW0001;
            bandedAgeCount.ZeroToTwelve = age.QS103EW0002 + age.QS103EW0003 + age.QS103EW0004 + age.QS103EW0005 + age.QS103EW0006 + age.QS103EW0007 + age.QS103EW0008 + age.QS103EW0009 + age.QS103EW0010 + age.QS103EW0011 + age.QS103EW0012 + age.QS103EW0013 + age.QS103EW0014;
            bandedAgeCount.ThirteenToEighteen = age.QS103EW0015 + age.QS103EW0016 + age.QS103EW0017 + age.QS103EW0018 + age.QS103EW0019 + age.QS103EW0020;
            bandedAgeCount.NineteenToThirty = age.QS103EW0021 + age.QS103EW0022 + age.QS103EW0023 + age.QS103EW0024 + age.QS103EW0025 + age.QS103EW0026 + age.QS103EW0027 + age.QS103EW0028 + age.QS103EW0029 + age.QS103EW0030 + age.QS103EW0031 + age.QS103EW0032;
            bandedAgeCount.ThirtyOneToFiftyNine = age.QS103EW0033 + age.QS103EW0034 + age.QS103EW0035 + age.QS103EW0036 + age.QS103EW0037 + age.QS103EW0038 + age.QS103EW0039 + age.QS103EW0040 + age.QS103EW0041 + age.QS103EW0042 + age.QS103EW0043 + age.QS103EW0044 + age.QS103EW0045 + age.QS103EW0046 + age.QS103EW0047 + age.QS103EW0049 + age.QS103EW0050 + age.QS103EW0051 + age.QS103EW0052 + age.QS103EW0053 + age.QS103EW0054 + age.QS103EW0055 + age.QS103EW0056 + age.QS103EW0057 + age.QS103EW0058 + age.QS103EW0059 + age.QS103EW0060 + age.QS103EW0061;
            bandedAgeCount.SixtyPlus = bandedAgeCount.All - (bandedAgeCount.ZeroToTwelve + bandedAgeCount.ThirteenToEighteen + bandedAgeCount.NineteenToThirty + bandedAgeCount.ThirtyOneToFiftyNine);

            return bandedAgeCount;
        }

        static BandedAgePercent BandAgesPercent(Age age)
        {
            BandedAgeCount bandedAge = new BandedAgeCount();

            bandedAge.All = age.QS103EW0001;
            bandedAge.ZeroToTwelve = age.QS103EW0002 + age.QS103EW0003 + age.QS103EW0004 + age.QS103EW0005 + age.QS103EW0006 + age.QS103EW0007 + age.QS103EW0008 + age.QS103EW0009 + age.QS103EW0010 + age.QS103EW0011 + age.QS103EW0012 + age.QS103EW0013 + age.QS103EW0014;
            bandedAge.ThirteenToEighteen = age.QS103EW0015 + age.QS103EW0016 + age.QS103EW0017 + age.QS103EW0018 + age.QS103EW0019 + age.QS103EW0020;
            bandedAge.NineteenToThirty = age.QS103EW0021 + age.QS103EW0022 + age.QS103EW0023 + age.QS103EW0024 + age.QS103EW0025 + age.QS103EW0026 + age.QS103EW0027 + age.QS103EW0028 + age.QS103EW0029 + age.QS103EW0030 + age.QS103EW0031 + age.QS103EW0032;
            bandedAge.ThirtyOneToFiftyNine = age.QS103EW0033 + age.QS103EW0034 + age.QS103EW0035 + age.QS103EW0036 + age.QS103EW0037 + age.QS103EW0038 + age.QS103EW0039 + age.QS103EW0040 + age.QS103EW0041 + age.QS103EW0042 + age.QS103EW0043 + age.QS103EW0044 + age.QS103EW0045 + age.QS103EW0046 + age.QS103EW0047 + age.QS103EW0049 + age.QS103EW0050 + age.QS103EW0051 + age.QS103EW0052 + age.QS103EW0053 + age.QS103EW0054 + age.QS103EW0055 + age.QS103EW0056 + age.QS103EW0057 + age.QS103EW0058 + age.QS103EW0059 + age.QS103EW0060 + age.QS103EW0061;
            bandedAge.SixtyPlus = bandedAge.All - (bandedAge.ZeroToTwelve + bandedAge.ThirteenToEighteen + bandedAge.NineteenToThirty + bandedAge.ThirtyOneToFiftyNine);

            BandedAgePercent bandedAgePercent = new BandedAgePercent();

            bandedAgePercent.ZeroToTwelvePercent = Math.Round(bandedAge.ZeroToTwelve / bandedAge.All, 3);
            bandedAgePercent.ThirteenToEighteenPercent = Math.Round(bandedAge.ThirteenToEighteen / bandedAge.All, 3);
            bandedAgePercent.NineteenToThirtyPercent = Math.Round(bandedAge.NineteenToThirty / bandedAge.All, 3);
            bandedAgePercent.ThirtyOneToFiftyNinePercent = Math.Round(bandedAge.ThirtyOneToFiftyNine / bandedAge.All, 3);
            bandedAgePercent.SixtyPlusPercent = Math.Round(bandedAge.SixtyPlus / bandedAge.All, 3);

            return bandedAgePercent;
        }

        static SocialGradeCount SocialGradesToCounts(SocialGrade socialGrade)
        {
            SocialGradeCount socialGradeCount = new SocialGradeCount();

            socialGradeCount.AllCount = socialGrade.QS611EW0001;
            socialGradeCount.ABCount = socialGrade.QS611EW0002;
            socialGradeCount.C1Count = socialGrade.QS611EW0003;
            socialGradeCount.C2Count = socialGrade.QS611EW0004;
            socialGradeCount.DECount = socialGrade.QS611EW0005;

            return socialGradeCount;
        }

        static SocialGradePercent SocialGradesToPercentages(SocialGrade socialGrade)
        {
            SocialGradePercent socialGradePercent = new SocialGradePercent();

            socialGradePercent.ABPercent = Math.Round(socialGrade.QS611EW0002 / socialGrade.QS611EW0001, 3);
            socialGradePercent.C1Percent = Math.Round(socialGrade.QS611EW0003 / socialGrade.QS611EW0001, 3);
            socialGradePercent.C2Percent = Math.Round(socialGrade.QS611EW0004 / socialGrade.QS611EW0001, 3);
            socialGradePercent.DEPercent = Math.Round(socialGrade.QS611EW0005 / socialGrade.QS611EW0001, 3);

            return socialGradePercent;
        }

        static string IncomeToIncomeClassification (IncomeAHC income, double lowercutoff, double uppercutoff)
        {
            if (income.CleanNetannualincomeafterhousingcosts <= lowercutoff)
            {
                return "Low income";
            }
            else if (income.CleanNetannualincomeafterhousingcosts >= uppercutoff)
            {
                return "High income";
            }
            else if (income.CleanNetannualincomeafterhousingcosts > lowercutoff && income.CleanNetannualincomeafterhousingcosts < uppercutoff)
            {
                return "Middle income";
            }
            else
            {
                return null;
            }
        }


        // Required output columns are,
        // OA = code 
        // Wealth = "Low Income, Middle Income, High Income"
        // Classification Group = My custom classifiers
        // Age profile = My custom classifiers
        // Largest job type = "AB, DE, etc..."
        // Ward Code = code
        // NUTS2 = code (UKE4 etc...)
        // Disposal Income(MSOA) = A double, weekly (annual/52)
        // Largest Group = Hmmm.... what is this?
        // 0-12% = easy
        // 13-18% = easy
        // 19-30% = easy
        // 31-59% = easy
        // 60+% = easy
        // AB = easy (percentage)
        // C1 = easy (percentage)
        // C2 = easy (percentage)
        // DE = easy (percentage)
        // White = easy (percentage)
        // PenPortrait = as text

        public class OANUTSLink
        {
            public string OA11CD { get; set; }
            public string LAU210CD { get; set; }
            public string LAU210NM { get; set; }
            public string OA11PERCENT { get; set; }
            public string LAU110CD { get; set; }
            public string LAU110NM { get; set; }
            public string OA11PERCENT1 { get; set; }
            public string NUTS306CD { get; set; }
            public string NUTS306NM { get; set; }
            public string OA11PERCENT2 { get; set; }
            public string NUTS206CD { get; set; }
            public string NUTS206NM { get; set; }
            public string OA11PERCENT3 { get; set; }
            public string NUTS106CD { get; set; }
            public string NUTS106NM { get; set; }
            public string OA11PERCENT4 { get; set; }
        }

        public class Baseline
        {
            public string Place { get; set; }
            public double Population { get; set; }
            public double MeanMedianIncomeAHC { get; set; }
            public double PercentWhite { get; set; }
            public PenPortraitCount PenPortraitCounts {get; set;}
            public IncomeCount IncomeCounts { get; set; }
            public AgeProfileCount AgeProfileCounts { get; set; }
            public CombinedProfileCount CombinedProfileCounts { get; set; }
            public BandedAgeCount BandedAges { get; set; }
            public SocialGradeCount SocialGrades { get; set; }
        }

        public class Event
        {
            public string OutputArea { get; set; }
            public int EventCount { get; set; }
        }


        public class OAInfo
        {
            public string OutputArea { get; set; }
            public double Population { get; set; }
            public double GraduateCount { get; set; }
            public double StudentCount { get; set; }
            public int TechEventCount { get; set; }
            public string MSOA { get; set; }
            public string WardCode { get; set; }
            public string WardName { get; set; }
            public string Localauthorityname { get; set; }
            public string NUTS2 { get; set; }
            public double MedianIncomeAHC { get; set; }
            public string IncomeGroup { get; set; }
            public double PercentWhite { get; set; }
            public string LargestSocialGrade { get; set; }
            public string PenPortrait { get; set; }
            public string AgeProfile { get; set; }
            public string CombinedProfile { get; set; }
            public BandedAgeCount BandedAgeCounts { get; set; }
            public SocialGradeCount SocialGradeCounts { get; set; }
            public BandedAgePercent BandedAgePercentages { get; set; }
            public SocialGradePercent SocialGradePercentages { get; set; }
        }

        public class SocialGrade
        {
            public string GeographyCode { get; set; }
            public double QS611EW0001 { get; set; } // All
            public double QS611EW0002 { get; set; } // AB
            public double QS611EW0003 { get; set; } // C1
            public double QS611EW0004 { get; set; } // C2
            public double QS611EW0005 { get; set; } // DE
        }

        public class SocialGradeCount
        {
            public double AllCount { get; set; } // All
            public double ABCount { get; set; } // AB
            public double C1Count { get; set; } // C1
            public double C2Count { get; set; } // C2
            public double DECount { get; set; } // DE
        }

        public class SocialGradePercent
        {
            public double ABPercent { get; set; }
            public double C1Percent { get; set; }
            public double C2Percent { get; set; }
            public double DEPercent { get; set; }
        }

        public class EthnicGroup
        {
            public string GeographyCode { get; set; }
            public double QS201EW0001 { get; set; } // All
            public double QS201EW0002 { get; set; } // White
            public double QS201EW0003 { get; set; }
            public double QS201EW0004 { get; set; }
            public double QS201EW0005 { get; set; }
            public double QS201EW0006 { get; set; }
            public double QS201EW0007 { get; set; }
            public double QS201EW0008 { get; set; }
            public double QS201EW0009 { get; set; }
            public double QS201EW0010 { get; set; }
            public double QS201EW0011 { get; set; }
            public double QS201EW0012 { get; set; }
            public double QS201EW0013 { get; set; }
            public double QS201EW0014 { get; set; }
            public double QS201EW0015 { get; set; }
            public double QS201EW0016 { get; set; }
            public double QS201EW0017 { get; set; }
            public double QS201EW0018 { get; set; }
            public double QS201EW0019 { get; set; }
        }

        public class BandedAgeCount
        {
            public double All { get; set; }
            public double ZeroToTwelve { get; set; }
            public double ThirteenToEighteen { get; set; }
            public double NineteenToThirty { get; set; }
            public double ThirtyOneToFiftyNine { get; set; }
            public double SixtyPlus { get; set; }
        }

        public class BandedAgePercent
        {
            public double ZeroToTwelvePercent { get; set; }
            public double ThirteenToEighteenPercent { get; set; }
            public double NineteenToThirtyPercent { get; set; }
            public double ThirtyOneToFiftyNinePercent { get; set; }
            public double SixtyPlusPercent { get; set; }
        }

        public class Skill
        {
            public string GeographyCode { get; set; }
            public double KS501EW0001 { get; set; } // All
            public double KS501EW0002 { get; set; } // No quals
            public double KS501EW0003 { get; set; } // Level 1 quals
            public double KS501EW0004 { get; set; } // Level 2
            public double KS501EW0005 { get; set; } // Apprenticeship
            public double KS501EW0006 { get; set; } // Level 3 quals
            public double KS501EW0007 { get; set; } // Level 4 of higher
            public double KS501EW0008 { get; set; } // Other quals
            public double KS501EW0009 { get; set; } // school kids 16-17
            public double KS501EW0010 { get; set; } // school kids 18+
            public double KS501EW0011 { get; set; } // students in employment
            public double KS501EW0012 { get; set; } // students unemployed
            public double KS501EW0013 { get; set; } // students not active
            public double KS501EW0014 { get; set; } // same as above but percentages
            public double KS501EW0015 { get; set; }
            public double KS501EW0016 { get; set; }
            public double KS501EW0017 { get; set; }
            public double KS501EW0018 { get; set; }
            public double KS501EW0019 { get; set; }
            public double KS501EW0020 { get; set; }
            public double KS501EW0021 { get; set; }
            public double KS501EW0022 { get; set; }
            public double KS501EW0023 { get; set; }
            public double KS501EW0024 { get; set; }
            public double KS501EW0025 { get; set; }
        }

        public class Age
        {
            public string GeographyCode { get; set; }
            public double QS103EW0001 { get; set; } // All
            public double QS103EW0002 { get; set; } // Under 1
            public double QS103EW0003 { get; set; } // 1
            public double QS103EW0004 { get; set; } // 2
            public double QS103EW0005 { get; set; }
            public double QS103EW0006 { get; set; }
            public double QS103EW0007 { get; set; }
            public double QS103EW0008 { get; set; }
            public double QS103EW0009 { get; set; }
            public double QS103EW0010 { get; set; }
            public double QS103EW0011 { get; set; }
            public double QS103EW0012 { get; set; }
            public double QS103EW0013 { get; set; }
            public double QS103EW0014 { get; set; }
            public double QS103EW0015 { get; set; }
            public double QS103EW0016 { get; set; }
            public double QS103EW0017 { get; set; }
            public double QS103EW0018 { get; set; }
            public double QS103EW0019 { get; set; }
            public double QS103EW0020 { get; set; }
            public double QS103EW0021 { get; set; }
            public double QS103EW0022 { get; set; }
            public double QS103EW0023 { get; set; }
            public double QS103EW0024 { get; set; }
            public double QS103EW0025 { get; set; }
            public double QS103EW0026 { get; set; }
            public double QS103EW0027 { get; set; }
            public double QS103EW0028 { get; set; }
            public double QS103EW0029 { get; set; }
            public double QS103EW0030 { get; set; }
            public double QS103EW0031 { get; set; }
            public double QS103EW0032 { get; set; }
            public double QS103EW0033 { get; set; }
            public double QS103EW0034 { get; set; }
            public double QS103EW0035 { get; set; }
            public double QS103EW0036 { get; set; }
            public double QS103EW0037 { get; set; }
            public double QS103EW0038 { get; set; }
            public double QS103EW0039 { get; set; }
            public double QS103EW0040 { get; set; }
            public double QS103EW0041 { get; set; }
            public double QS103EW0042 { get; set; }
            public double QS103EW0043 { get; set; }
            public double QS103EW0044 { get; set; }
            public double QS103EW0045 { get; set; }
            public double QS103EW0046 { get; set; }
            public double QS103EW0047 { get; set; }
            public double QS103EW0048 { get; set; }
            public double QS103EW0049 { get; set; }
            public double QS103EW0050 { get; set; }
            public double QS103EW0051 { get; set; }
            public double QS103EW0052 { get; set; }
            public double QS103EW0053 { get; set; }
            public double QS103EW0054 { get; set; }
            public double QS103EW0055 { get; set; }
            public double QS103EW0056 { get; set; }
            public double QS103EW0057 { get; set; }
            public double QS103EW0058 { get; set; }
            public double QS103EW0059 { get; set; }
            public double QS103EW0060 { get; set; }
            public double QS103EW0061 { get; set; }
            public double QS103EW0062 { get; set; }
            public double QS103EW0063 { get; set; }
            public double QS103EW0064 { get; set; }
            public double QS103EW0065 { get; set; }
            public double QS103EW0066 { get; set; }
            public double QS103EW0067 { get; set; }
            public double QS103EW0068 { get; set; }
            public double QS103EW0069 { get; set; }
            public double QS103EW0070 { get; set; }
            public double QS103EW0071 { get; set; }
            public double QS103EW0072 { get; set; }
            public double QS103EW0073 { get; set; }
            public double QS103EW0074 { get; set; }
            public double QS103EW0075 { get; set; }
            public double QS103EW0076 { get; set; }
            public double QS103EW0077 { get; set; }
            public double QS103EW0078 { get; set; }
            public double QS103EW0079 { get; set; }
            public double QS103EW0080 { get; set; }
            public double QS103EW0081 { get; set; }
            public double QS103EW0082 { get; set; }
            public double QS103EW0083 { get; set; }
            public double QS103EW0084 { get; set; }
            public double QS103EW0085 { get; set; }
            public double QS103EW0086 { get; set; }
            public double QS103EW0087 { get; set; }
            public double QS103EW0088 { get; set; }
            public double QS103EW0089 { get; set; }
            public double QS103EW0090 { get; set; }
            public double QS103EW0091 { get; set; }
            public double QS103EW0092 { get; set; }
            public double QS103EW0093 { get; set; }
            public double QS103EW0094 { get; set; }
            public double QS103EW0095 { get; set; }
            public double QS103EW0096 { get; set; }
            public double QS103EW0097 { get; set; }
            public double QS103EW0098 { get; set; }
            public double QS103EW0099 { get; set; }
            public double QS103EW0100 { get; set; }
            public double QS103EW0101 { get; set; } // 99
            public double QS103EW0102 { get; set; } // 100 and over
        }

        public class AgeProfileCount
        {
            public double StudentsYoungPros { get; set; }
            public double YoungFamily { get; set; }
            public double OlderFamily { get; set; }
            public double OlderPeople { get; set; }
        }

        public class PenPortrait
        {
            public string OutputAreaCode { get; set; }
            public string LocalAuthorityCode { get; set; }
            public string LocalAuthorityName { get; set; }
            public string RegionCountryCode { get; set; }
            public string RegionCountryName { get; set; }
            public string SupergroupCode { get; set; }
            public string SupergroupName { get; set; }
            public string GroupCode { get; set; }
            public string GroupName { get; set; }
            public string SubgroupCode { get; set; }
            public string SubgroupName { get; set; }
        }

        public sealed class PenPortraitMap : ClassMap<PenPortrait>
        {
            public PenPortraitMap()
            {
                Map(m => m.OutputAreaCode).Name("Output Area Code");
                Map(m => m.LocalAuthorityCode).Name("Local Authority Code");
                Map(m => m.LocalAuthorityName).Name("Local Authority Name");
                Map(m => m.RegionCountryCode).Name("Region/Country Code");
                Map(m => m.RegionCountryName).Name("Region/Country Name");
                Map(m => m.SupergroupCode).Name("Supergroup Code");
                Map(m => m.SupergroupName).Name("Supergroup Name");
                Map(m => m.GroupCode).Name("Group Code");
                Map(m => m.GroupName).Name("Group Name");
                Map(m => m.SubgroupCode).Name("Subgroup Code");
                Map(m => m.SubgroupName).Name("Subgroup Name");
            }
        }

        public class PenPortraitCount
        {
            public double ConstrainedCityDwellers { get; set; }
            public double Cosmopolitans { get; set; }
            public double EthnicityCentral { get; set; }
            public double HardPressedLiving { get; set; }
            public double MulticulturalMetropolitans { get; set; }
            public double RuralResidents { get; set; }
            public double Suburbanites { get; set; }
            public double Urbanites { get; set; }
        }

        public class CombinedProfileCount
        {
            public double StudentsYoungPros { get; set; }
            public double PoorerYoungFamily { get; set; }
            public double WealthyYoungFamily { get; set; }
            public double PoorerOlderFamily { get; set; }
            public double WealthyOlderFamily { get; set; }
            public double PoorerOlderPeople { get; set; }
            public double WealthyOlderPeople { get; set; }
        }

        public class IncomeAHC
        {
            public string MSOAcode { get; set; }
            public string MSOAname { get; set; }
            public string Localauthoritycode { get; set; }
            public string Localauthorityname { get; set; }
            public string Regioncode { get; set; }
            public string Regionname { get; set; }
            public string Netannualincomeafterhousingcosts { get; set; }
            public double CleanNetannualincomeafterhousingcosts { get; set; }
            public string Upperconfidencelimit { get; set; }
            public string Lowerconfidencelimit { get; set; }
            public string Confidenceinterval { get; set; }
        }

        public class IncomeCount
        {
            public double LowIncomeCount { get; set; }
            public double MiddleIncomeCount { get; set; }
            public double HighIncomeCount { get; set; }
        }

        public sealed class IncomeMap : ClassMap<IncomeAHC>
        {
            public IncomeMap()
            {
                Map(m => m.MSOAcode).Name("MSOA code");
                Map(m => m.MSOAname).Name("MSOA name");
                Map(m => m.Localauthoritycode).Name("Local authority code");
                Map(m => m.Localauthorityname).Name("Local authority name");
                Map(m => m.Regioncode).Name("Region code");
                Map(m => m.Regionname).Name("Region name");
                Map(m => m.Netannualincomeafterhousingcosts).Name("Net annual income after housing costs (£)"); 
                Map(m => m.Upperconfidencelimit).Name("Upper confidence limit (£)");
                Map(m => m.Lowerconfidencelimit).Name("Lower confidence limit (£)");
                Map(m => m.Confidenceinterval).Name("Confidence interval (£)");
            }
        }
        
        public class OAMSOALink
        {
            public string OA11CD { get; set; }
            public string LAD16CD { get; set; }
            public string LAD16NM { get; set; }
            public string LSOA11CD { get; set; }
            public string LSOA11NM { get; set; }
            public string MSOA11CD { get; set; }
            public string MSOA11NM { get; set; }
            public string LEP17CD1 { get; set; }
            public string LEP17NM1 { get; set; }
            public string LEP17CD2 { get; set; }
            public string LEP17NM2 { get; set; }
            public string FID { get; set; }
        }        

        public class OALink
        {
            public string OA11CD { get; set; }
            public string WD17CD { get; set; }
            public string WD17NM { get; set; }
            public string WD17NMW { get; set; }
            public string LAD17CD { get; set; }
            public string LAD17NM { get; set; }
            public string FID { get; set; }
        }

    }
}
