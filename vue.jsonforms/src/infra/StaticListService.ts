import moment from "moment";
import { v4 as uuid } from "uuid";
import { DisclosureEntity } from "@/entities/stationery/DisclosureEntity";
import { DocumentFormatting } from "@/entities/stationery/DocumentFormatting";
import { MediaPlatform } from "@/entities/MediaPlatform";
import { StationeryEntity } from "@/entities/stationery/StationeryEntity";
import { StationeryStatus } from "@/entities/stationery/StationeryStatus";
import { ArStatusesModel } from "@/pages/models/appointed-representative/ArStatusesModel";
import { ArProductTypesModel } from "@/pages/models/appointed-representative/ArProductTypesModel";
import { ArProductsModel } from "@/pages/models/appointed-representative/ArProductsModel";
import { ArActivityProductsModel } from "@/pages/models/appointed-representative/ArActivityProductsModel";
import { ArMarketServiceModel } from "@/pages/models/appointed-representative/ArMarketServiceModel";

export default class StaticList {
  static getFirmTypes(): string[] {
    return [
      "Private Limited Company",
      "Public Limited Company",
      "Company Limited by Guarantee",
      "Overseas Firm Incorporated",
      "Registered as Overseas Company by Companies House",
      "Limited Liability Partnership",
      "Partnership (other than the above)",
      "Trust",
      "+Add Manually",
    ];
  }

  static getTitles(): string[] {
    return ["Mr", "Mrs", "Miss", "Ms", "Dr"];
  }

  static getNationalities(): string[] {
    return [
      "Afghan",
      "Algerian",
      "American",
      "Angolan",
      "Antiguan",
      "Argentinian",
      "Argentinian",
      "Australian",
      "Austrian",
      "Bahamian",
      "Bangladeshi",
      "Barbadian",
      "Belgian",
      "Beninese",
      "Bolivian",
      "Bolivian",
      "Botswanan",
      "Brazilian",
      "Brazilian",
      "British",
      "Burkinabé",
      "Cambodian",
      "Cameroonian",
      "Canadian",
      "Central African",
      "Chadian",
      "Chadian",
      "Chilean",
      "Chilean",
      "Chinese",
      "Colombian",
      "Colombian",
      "Comorian",
      "Costa Rican",
      "Cuban",
      "Czech",
      "Danish",
      "Dominican",
      "Dutch",
      "Ecuadorian",
      "Ecuadorian",
      "Egyptian",
      "Emirati",
      "Equatorial Guinean",
      "Eritrean",
      "Ethiopian",
      "Filipino",
      "Finnish",
      "French",
      "Gabonese",
      "Gabonese",
      "German",
      "Ghanaian",
      "Greek",
      "Guatemalan",
      "Guinean",
      "Guinean",
      "Guyanese",
      "Haitian",
      "Honduran",
      "Hungarian",
      "Indian",
      "Indonesian",
      "Iranian",
      "Iraqi",
      "Irish",
      "Israeli",
      "Italian",
      "Ivorian",
      "Jamaican",
      "Japanese",
      "Jordanian",
      "Kenyan",
      "Kenyan",
      "Korean",
      "Laotian",
      "Lebanese",
      "Lesotho",
      "Liberian",
      "Libyan",
      "Malagasy",
      "Malawian",
      "Malaysian",
      "Malian",
      "Malian",
      "Malian",
      "Mauritanian",
      "Mauritanian",
      "Mauritanian",
      "Mauritanian",
      "Mexican",
      "Mongolian",
      "Moroccan",
      "Mozambican",
      "Myanmar",
      "Namibian",
      "Namibian",
      "Namibian",
      "Nepali",
      "New Zealander",
      "Nicaraguan",
      "Nigerian",
      "Nigerien",
      "Nigerien",
      "North Korean",
      "Norwegian",
      "Omani",
      "Pakistani",
      "Panamanian",
      "Paraguayan",
      "Paraguayan",
      "Peruvian",
      "Peruvian",
      "Polish",
      "Portuguese",
      "Puerto Rican",
      "Qatari",
      "Romanian",
      "Russian",
      "Rwandan",
      "Rwandan",
      "Salvadoran",
      "Saudi Arabian",
      "Senegalese",
      "Senegalese",
      "Seychellois",
      "Sierra Leonean",
      "Sierra Leonean",
      "Singaporean",
      "Somali",
      "South African",
      "South African",
      "South Korean",
      "South Sudanese",
      "Spanish",
      "Sri Lankan",
      "Sudanese",
      "Sudanese",
      "Surinamese",
      "Swazi",
      "Swazi",
      "Swedish",
      "Swiss",
      "Syrian",
      "São Toméan",
      "São Toméan",
      "Tanzanian",
      "Tanzanian",
      "Thai",
      "Togolese",
      "Togolese",
      "Togolese",
      "Trinidadian",
      "Tunisian",
      "Turkish",
      "Ugandan",
      "Ugandan",
      "Ukrainian",
      "Uruguayan",
      "Uruguayan",
      "Venezuelan",
      "Vietnamese",
      "Yemeni",
      "Zambian",
      "Zambian",
      "Zimbabwean",
      "Zimbabwean",
    ];
  }

  static getRoles(): string[] {
    return [
      "Director (SMF3)",
      "Compliance Oversight (SMF16)",
      "Anti Money Laundering (SMF17)",
      "Responsible for Mortgage Intermediation of the Firm",
      "Responsible for Insurance Distribution of the Firm",
      "Responsible for Complaints Handling of the Firm",
      "Manager of Certification Employee(s) (i.e. Supervisor)",
      "Mortgage Broker",
      "Insurance Broker",
      "Protection Broker",
    ];
  }

  static getFinancialSolvency(): string[] {
    return ["Financially Solvent", "Financially Insolvent"];
  }

  static getPiiBusinessLinesCovered(): string[] {
    return [
      "Mortgage advising and arranging",
      "Non-investment Insurance Advising / Arranging / Dealing / Assisting",
      "Retail investment advising / arranging",
    ];
  }

  static getPiiTimePeriodOfExclusion(): string[] {
    return ["Past business", "Future business", "Past and future business"];
  }

  static getPiiTypesOfExclusion(): string[] {
    return [
      "All business lines",
      "Volume of business",
      "Type of consumer",
      "Type of business",
      "Sub-limit of cover",
      "Jurisdiction of insurers used",
      "Rating of insurer used",
      "Other",
    ];
  }

  // NOTE. Below methods actually have the same implementation except for getPiiBusinessLineCategoriesSubjectToPolicyExclusions
  // which differs for having 'No exclusions apply to this policy' instead of 'All business lines'.
  // TODO. to consolidate later if duplication is not necessary
  static getPiiBusinessLinesSubjectToLimitOfIndemnitySingle(
    businessLineCoveredSelected: string[]
  ): string[] {
    const list: string[] = ["All business lines"];

    businessLineCoveredSelected.forEach((businessLine) => {
      switch (businessLine) {
        case "Mortgage advising and arranging": {
          const additionalList: string[] = [
            "Mortgages - Standard/general",
            "Mortgages - Impaired credit",
            "Mortgages - Self certification",
            "Mortgages - Endowments",
            "Mortgages - Equity Release",
            "Mortgages - Other Mortgage Type",
          ];

          list.push(...additionalList);
          break;
        }
        case "Non-investment Insurance Advising / Arranging / Dealing / Assisting": {
          const additionalList: string[] = [
            "General Insurance and pure protection - Standard/general",
            "General Insurance and pure protection - Commercial",
            "General Insurance and pure protection - Critical illness",
            "General Insurance and pure protection - Income protection",
            "General Insurance and pure protection - Delegated Authority Business",
            "General Insurance and pure protection - Other GI and Pure protection type",
          ];

          list.push(...additionalList);
          break;
        }
        default: {
          const additionaList: string[] = [
            "Retail Investments - Standard/general",
            "Retail Investments - Income drawdown / withdrawal",
            "Retail Investments - Investment bonds",
            "Retail Investments - Personal pensions and AVCs",
            "Retail Investments - Structured products",
            "Retail Investments - DB pension transfers/safeguarded benefits",
            "Retail investments - NMPI/NRRS",
            "Retail investments - Other retail investment type",
          ];

          list.push(...additionaList);
        }
      }
    });

    const additionalDefaultList: string[] = [
      "Other FCA regulated business",
      "All others",
    ];

    list.push(...additionalDefaultList);
    return list;
  }

  static getPiiBusinessLinesSubjectToLimitOfIndemnityAggregate(
    businessLineCoveredSelected: string[]
  ): string[] {
    const list: string[] = ["All business lines"];

    businessLineCoveredSelected.forEach((businessLine) => {
      switch (businessLine) {
        case "Mortgage advising and arranging": {
          const additionaList: string[] = [
            "Mortgages - Standard/general",
            "Mortgages - Impaired credit",
            "Mortgages - Self certification",
            "Mortgages - Endowments",
            "Mortgages - Equity Release",
            "Mortgages - Other Mortgage Type",
          ];

          list.push(...additionaList);
          break;
        }
        case "Non-investment Insurance Advising / Arranging / Dealing / Assisting": {
          const additionalList: string[] = [
            "General Insurance and pure protection - Standard/general",
            "General Insurance and pure protection - Commercial",
            "General Insurance and pure protection - Critical illness",
            "General Insurance and pure protection - Income protection",
            "General Insurance and pure protection - Delegated Authority Business",
            "General Insurance and pure protection - Other GI and Pure protection type",
          ];

          list.push(...additionalList);
          break;
        }
        default: {
          const additionaList: string[] = [
            "Retail Investments - Standard/general",
            "Retail Investments - Income drawdown / withdrawal",
            "Retail Investments - Investment bonds",
            "Retail Investments - Personal pensions and AVCs",
            "Retail Investments - Structured products",
            "Retail Investments - DB pension transfers/safeguarded benefits",
            "Retail investments - NMPI/NRRS",
            "Retail investments - Other retail investment type",
          ];

          list.push(...additionaList);
        }
      }
    });

    const additionalDefaultList: string[] = [
      "Other FCA regulated business",
      "All others",
    ];

    list.push(...additionalDefaultList);
    return list;
  }

  static getPiiBusinessLineCategoriesSubjectToPolicyExclusions(
    businessLineCoveredSelected: string[]
  ): string[] {
    const list: string[] = ["No exclusions apply to this policy"];

    businessLineCoveredSelected.forEach((businessLine) => {
      switch (businessLine) {
        case "Mortgage advising and arranging": {
          const additionalList: string[] = [
            "Mortgages - Standard/general",
            "Mortgages - Impaired credit",
            "Mortgages - Self certification",
            "Mortgages - Endowments",
            "Mortgages - Equity Release",
            "Mortgages - Other Mortgage Type",
          ];

          list.push(...additionalList);
          break;
        }
        case "Non-investment Insurance Advising / Arranging / Dealing / Assisting": {
          const additionalList: string[] = [
            "General insurance and pure protection - Standard/general",
            "General insurance and pure protection - Commercial",
            "General insurance and pure protection - Critical illness",
            "General insurance and pure protection - Income Protection",
            "General insurance and pure protection - Delegated Authority Business",
            "General insurance and pure protection - Other GI and Pure protection type",
          ];

          list.push(...additionalList);
          break;
        }
        default: {
          const additionalList: string[] = [
            "Retail Investments - Standard/general",
            "Retail Investments - Income drawdown / withdrawal",
            "Retail Investments - Investment bonds",
            "Retail Investments - Personal pensions and AVCs",
            "Retail Investments - Structured products",
            "Retail Investments - DB pension transfers/safeguarded benefits",
            "Retail Investments - NMPI/NRRS",
            "Retail Investments - Other retail investment type",
          ];

          list.push(...additionalList);
        }
      }
    });

    const additionalDefaultList: string[] = [
      "Other FCA regulated business",
      "All other",
    ];

    list.push(...additionalDefaultList);
    return list;
  }

  static getPiiBusinessLineCategoriesSubjectToPolicyExcess(
    businessLineCoveredSelected: string[]
  ): string[] {
    const list: string[] = ["All business lines"];

    businessLineCoveredSelected.forEach((businessLine) => {
      switch (businessLine) {
        case "Mortgage advising and arranging": {
          const additionalList: string[] = [
            "Mortgages - Standard/general",
            "Mortgages - Impaired credit",
            "Mortgages - Self certification",
            "Mortgages - Endowments",
            "Mortgages - Equity Release",
            "Mortgages - Other Mortgage Type",
          ];

          list.push(...additionalList);
          break;
        }
        case "Non-investment Insurance Advising / Arranging / Dealing / Assisting": {
          const additionalList: string[] = [
            "General Insurance and pure protection - Standard/general",
            "General Insurance and pure protection - Commercial",
            "General Insurance and pure protection - Critical illness",
            "General Insurance and pure protection - Income protection",
            "General Insurance and pure protection - Delegated Authority Business",
            "General Insurance and pure protection - Other GI and Pure protection type",
          ];

          list.push(...additionalList);
          break;
        }
        default: {
          const additionalList: string[] = [
            "Retail Investments - Standard/general",
            "Retail Investments - Income drawdown / withdrawal",
            "Retail Investments - Investment bonds",
            "Retail Investments - Personal pensions and AVCs",
            "Retail Investments - Structured products",
            "Retail Investments - DB pension transfers/safeguarded benefits",
            "Retail investments - NMPI/NRRS",
            "Retail investments - Other retail investment type",
          ];

          list.push(...additionalList);
        }
      }
    });

    const additionalDefaultList: string[] = [
      "Other FCA regulated business",
      "All other",
    ];

    list.push(...additionalDefaultList);
    return list;
  }

  static getPiiInsurerNames(): string[] {
    return [
      "Acapella Syndicate 2014 (Managed by Pembroke Managing Agency Limited",
      "Ace",
      "Aegis Sybdicate 1225 at Lloyd's",
      "AIG Europe Ltd",
      "American International Group (AIG)",
      "Amtrust at Lloyd's 1861",
      "AmTrust Europe Limited",
      "Antares Syndicate 1274",
      "Arch Insurance Company (Europe) Ltd",
      "Arch Underwriting",
      "Aviva",
      "AXA Insurance UK",
      "Axis Specialty Europe SE / Axis Syndicate 1586 at Lloyd's",
      "Beazley (Lloyd's Syndicate or Limited Company)",
      "Brit (Lloyd's Syndicate or Limited Company)",
      "Canopius Managing Agents (Previously Trenwick)",
      "Catlin Insurance Company Ltd",
      "Channel Syndicate at Lloyd's 2015",
      "Chartis UK",
      "Chaucer Insurance Company",
      "Chubb European Group SE",
      "CAN Insurance",
      "DCH Sybdicate at Lloyd's 386",
      "DTW Syndicate at Loyds's 1991",
      "DUAL Corporate Risks",
      "Eureko Insurance Ireland Ltd",
      "Everest at Lloyd's 2786",
      "Goldate Insurance Company",
      "Greate Lakes Insurance SE (UK Branch)",
      "HCC (Lloyd's Syndicate)",
      "HCC Internation Insurance Company Plc",
      "HDI Global Specialty SE",
      "Hiscox (Lloyd's Syndicate or Limited Company)",
      "Liberty Managing Agency limited (4472 ; 5381)",
      "Liberty Manual Insurance Europe",
      "Markel (Lloyd's Sybdicate)",
      "Markel International Insurance Company Ltd",
      "MS Amlin",
      "MS Amlin syndicate 2001",
      "Munich Re Syndicate at Lloyd's 457",
      "Named Underwirters at Lloyd's",
      "Navigators Syndicate at Lloyd's",
      "Neon Syndicate at Lloyd's 2468",
      "Omnyy LLP",
      "Other",
      "Probitas Syndicate at Lloyd's 1492",
      "QBE International Insurance Limited",
      "Royal and Sun Alliance plc",
      "The Griffin Insurance Association Limited",
      "Travelers Insurance Company",
      "W R Berkley Syndicate at Lloyd's 1967",
      "XL Insurance Company SE",
      "Zurich Insurance plc (Branch of overseas firm)",
      "Allianz Global Corporate and Specialty SE",
      "China Re Syndicate at Lloyd's 2088",
      "Pembroke Syndicate at Lloyd's 4000",
      "International General Insurance Company (UK) Ltd (IGI)",
      "QIC Europe Limited",
      "Sompo International Insurance Ltd",
      "Starr International (Europe) Ltd",
      "Starr Managing Agents Limited",
      "Travelers Insurance DAC",
      "Travelers at Lloyd's 5000",
      "XL Insurance Company UK Limited",
    ];
  }

  static getProductStatusItems(): string[] {
    return ["Authorised", "Not Authorised", "Unregulated Activity"];
  }

  static getAppointReasons(): string[] {
    return [
      "Compliance Services",
      "Regulatory Hosting Services",
      "Comission",
      "IT Services",
    ];
  }

  static getServicesARPayFor(): string[] {
    return [
      "Distribution of Products",
      "Services Investment Adviser to Fund Managed by principal",
      "Connected Firm",
      "Hosting",
      "Compliance Services",
      "Incubation acquisition of an appointed representative",
      "Restructuring of business",
      "Introductions",
      "Capital raising for principal business",
    ];
  }

  static getFeesCharges(): string[] {
    return [
      "Initial Setup Fee",
      "Fixed Monthly Fee",
      "Percentage of Regulated Annual Income",
      "Percentage of Non-Regulated Annual Income",
      "Percentage of Regulated Annual Commission Income",
      "Percentage of Non-Regulated Annual Commission Income",
    ];
  }

  static getNatureOfBusinesses(): string[] {
    // reference: https://resources.companieshouse.gov.uk/sic/
    return [
      "01110 Growing of cereals (except rice), leguminous crops and oil seeds",
      "01120 Growing of rice",
      "01130 Growing of vegetables and melons, roots and tubers",
      "01140 Growing of sugar cane",
      "01150 Growing of tobacco",
      "01160 Growing of fibre crops",
      "01190 Growing of other non-perennial crops",
      "01210 Growing of grapes",
      "01220 Growing of tropical and subtropical fruits",
      "01230 Growing of citrus fruits",
      "01240 Growing of pome fruits and stone fruits",
      "01250 Growing of other tree and bush fruits and nuts",
      "01260 Growing of oleaginous fruits",
      "01270 Growing of beverage crops",
      "01280 Growing of spices, aromatic, drug and pharmaceutical crops",
      "01290 Growing of other perennial crops",
      "01300 Plant propagation",
      "01410 Raising of dairy cattle",
      "01420 Raising of other cattle and buffaloes",
      "01430 Raising of horses and other equines",
      "01440 Raising of camels and camelids",
      "01450 Raising of sheep and goats",
      "01460 Raising of swine/pigs",
      "01470 Raising of poultry",
      "01490 Raising of other animals",
      "01500 Mixed farming",
      "01610 Support activities for crop production",
      "01621 Farm animal boarding and care",
      "01629 Support activities for animal production (other than farm animal boarding and care) n.e.c.",
      "01630 Post-harvest crop activities",
      "01640 Seed processing for propagation",
      "01700 Hunting, trapping and related service activities",
      "02100 Silviculture and other forestry activities",
      "02200 Logging",
      "02300 Gathering of wild growing non-wood products",
      "02400 Support services to forestry",
      "03110 Marine fishing",
      "03120 Freshwater fishing",
      "03210 Marine aquaculture",
      "03220 Freshwater aquaculture",
      "05101 Deep coal mines",
      "05102 Open cast coal working",
      "05200 Mining of lignite",
      "06100 Extraction of crude petroleum",
      "06200 Extraction of natural gas",
      "07100 Mining of iron ores",
      "07210 Mining of uranium and thorium ores",
      "07290 Mining of other non-ferrous metal ores",
      "08110 Quarrying of ornamental and building stone, limestone, gypsum, chalk and slate",
      "08120 Operation of gravel and sand pits; mining of clays and kaolin",
      "08910 Mining of chemical and fertilizer minerals",
      "08920 Extraction of peat",
      "08930 Extraction of salt",
      "08990 Other mining and quarrying n.e.c.",
      "09100 Support activities for petroleum and natural gas mining",
      "09900 Support activities for other mining and quarrying",
      "10110 Processing and preserving of meat",
      "10120 Processing and preserving of poultry meat",
      "10130 Production of meat and poultry meat products",
      "10200 Processing and preserving of fish, crustaceans and molluscs",
      "10310 Processing and preserving of potatoes",
      "10320 Manufacture of fruit and vegetable juice",
      "10390 Other processing and preserving of fruit and vegetables",
      "10410 Manufacture of oils and fats",
      "10420 Manufacture of margarine and similar edible fats",
      "10511 Liquid milk and cream production",
      "10512 Butter and cheese production",
      "10519 Manufacture of other milk products",
      "10520 Manufacture of ice cream",
      "10611 Grain milling",
      "10612 Manufacture of breakfast cereals and cereals-based food",
      "10620 Manufacture of starches and starch products",
      "10710 Manufacture of bread; manufacture of fresh pastry goods and cakes",
      "10720 Manufacture of rusks and biscuits; manufacture of preserved pastry goods and cakes",
      "10730 Manufacture of macaroni, noodles, couscous and similar farinaceous products",
      "10810 Manufacture of sugar",
      "10821 Manufacture of cocoa and chocolate confectionery",
      "10822 Manufacture of sugar confectionery",
      "10831 Tea processing",
      "10832 Production of coffee and coffee substitutes",
      "10840 Manufacture of condiments and seasonings",
      "10850 Manufacture of prepared meals and dishes",
      "10860 Manufacture of homogenized food preparations and dietetic food",
      "10890 Manufacture of other food products n.e.c.",
      "10910 Manufacture of prepared feeds for farm animals",
      "10920 Manufacture of prepared pet foods",
      "11010 Distilling, rectifying and blending of spirits",
      "11020 Manufacture of wine from grape",
      "11030 Manufacture of cider and other fruit wines",
      "11040 Manufacture of other non-distilled fermented beverages",
      "11050 Manufacture of beer",
      "11060 Manufacture of malt",
      "11070 Manufacture of soft drinks; production of mineral waters and other bottled waters",
      "12000 Manufacture of tobacco products",
      "13100 Preparation and spinning of textile fibres",
      "13200 Weaving of textiles",
      "13300 Finishing of textiles",
      "13910 Manufacture of knitted and crocheted fabrics",
      "13921 Manufacture of soft furnishings",
      "13922 manufacture of canvas goods, sacks, etc.",
      "13923 manufacture of household textiles",
      "13931 Manufacture of woven or tufted carpets and rugs",
      "13939 Manufacture of other carpets and rugs",
      "13940 Manufacture of cordage, rope, twine and netting",
      "13950 Manufacture of non-wovens and articles made from non-wovens, except apparel",
      "13960 Manufacture of other technical and industrial textiles",
      "13990 Manufacture of other textiles n.e.c.",
      "14110 Manufacture of leather clothes",
      "14120 Manufacture of workwear",
      "14131 Manufacture of other men's outerwear",
      "14132 Manufacture of other women's outerwear",
      "14141 Manufacture of men's underwear",
      "14142 Manufacture of women's underwear",
      "14190 Manufacture of other wearing apparel and accessories n.e.c.",
      "14200 Manufacture of articles of fur",
      "14310 Manufacture of knitted and crocheted hosiery",
      "14390 Manufacture of other knitted and crocheted apparel",
      "15110 Tanning and dressing of leather; dressing and dyeing of fur",
      "15120 Manufacture of luggage, handbags and the like, saddlery and harness",
      "15200 Manufacture of footwear",
      "16100 Sawmilling and planing of wood",
      "16210 Manufacture of veneer sheets and wood-based panels",
      "16220 Manufacture of assembled parquet floors",
      "16230 Manufacture of other builders' carpentry and joinery",
      "16240 Manufacture of wooden containers",
      "16290 Manufacture of other products of wood; manufacture of articles of cork, straw and plaiting materials",
      "17110 Manufacture of pulp",
      "17120 Manufacture of paper and paperboard",
      "17211 Manufacture of corrugated paper and paperboard, sacks and bags",
      "17219 Manufacture of other paper and paperboard containers",
      "17220 Manufacture of household and sanitary goods and of toilet requisites",
      "17230 Manufacture of paper stationery",
      "17240 Manufacture of wallpaper",
      "17290 Manufacture of other articles of paper and paperboard n.e.c.",
      "18110 Printing of newspapers",
      "18121 Manufacture of printed labels",
      "18129 Printing n.e.c.",
      "18130 Pre-press and pre-media services",
      "18140 Binding and related services",
      "18201 Reproduction of sound recording",
      "18202 Reproduction of video recording",
      "18203 Reproduction of computer media",
      "19100 Manufacture of coke oven products",
      "19201 Mineral oil refining",
      "19209 Other treatment of petroleum products (excluding petrochemicals manufacture)",
      "20110 Manufacture of industrial gases",
      "20120 Manufacture of dyes and pigments",
      "20130 Manufacture of other inorganic basic chemicals",
      "20140 Manufacture of other organic basic chemicals",
      "20150 Manufacture of fertilizers and nitrogen compounds",
      "20160 Manufacture of plastics in primary forms",
      "20170 Manufacture of synthetic rubber in primary forms",
      "20200 Manufacture of pesticides and other agrochemical products",
      "20301 Manufacture of paints, varnishes and similar coatings, mastics and sealants",
      "20302 Manufacture of printing ink",
      "20411 Manufacture of soap and detergents",
      "20412 Manufacture of cleaning and polishing preparations",
      "20420 Manufacture of perfumes and toilet preparations",
      "20510 Manufacture of explosives",
      "20520 Manufacture of glues",
      "20530 Manufacture of essential oils",
      "20590 Manufacture of other chemical products n.e.c.",
      "20600 Manufacture of man-made fibres",
      "21100 Manufacture of basic pharmaceutical products",
      "21200 Manufacture of pharmaceutical preparations",
      "22110 Manufacture of rubber tyres and tubes; retreading and rebuilding of rubber tyres",
      "22190 Manufacture of other rubber products",
      "22210 Manufacture of plastic plates, sheets, tubes and profiles",
      "22220 Manufacture of plastic packing goods",
      "22230 Manufacture of builders ware of plastic",
      "22290 Manufacture of other plastic products",
      "23110 Manufacture of flat glass",
      "23120 Shaping and processing of flat glass",
      "23130 Manufacture of hollow glass",
      "23140 Manufacture of glass fibres",
      "23190 Manufacture and processing of other glass, including technical glassware",
      "23200 Manufacture of refractory products",
      "23310 Manufacture of ceramic tiles and flags",
      "23320 Manufacture of bricks, tiles and construction products, in baked clay",
      "23410 Manufacture of ceramic household and ornamental articles",
      "23420 Manufacture of ceramic sanitary fixtures",
      "23430 Manufacture of ceramic insulators and insulating fittings",
      "23440 Manufacture of other technical ceramic products",
      "23490 Manufacture of other ceramic products n.e.c.",
      "23510 Manufacture of cement",
      "23520 Manufacture of lime and plaster",
      "23610 Manufacture of concrete products for construction purposes",
      "23620 Manufacture of plaster products for construction purposes",
      "23630 Manufacture of ready-mixed concrete",
      "23640 Manufacture of mortars",
      "23650 Manufacture of fibre cement",
      "23690 Manufacture of other articles of concrete, plaster and cement",
      "23700 Cutting, shaping and finishing of stone",
      "23910 Production of abrasive products",
      "23990 Manufacture of other non-metallic mineral products n.e.c.",
      "24100 Manufacture of basic iron and steel and of ferro-alloys",
      "24200 Manufacture of tubes, pipes, hollow profiles and related fittings, of steel",
      "24310 Cold drawing of bars",
      "24320 Cold rolling of narrow strip",
      "24330 Cold forming or folding",
      "24340 Cold drawing of wire",
      "24410 Precious metals production",
      "24420 Aluminium production",
      "24430 Lead, zinc and tin production",
      "24440 Copper production",
      "24450 Other non-ferrous metal production",
      "24460 Processing of nuclear fuel",
      "24510 Casting of iron",
      "24520 Casting of steel",
      "24530 Casting of light metals",
      "24540 Casting of other non-ferrous metals",
      "25110 Manufacture of metal structures and parts of structures",
      "25120 Manufacture of doors and windows of metal",
      "25210 Manufacture of central heating radiators and boilers",
      "25290 Manufacture of other tanks, reservoirs and containers of metal",
      "25300 Manufacture of steam generators, except central heating hot water boilers",
      "25400 Manufacture of weapons and ammunition",
      "25500 Forging, pressing, stamping and roll-forming of metal; powder metallurgy",
      "25610 Treatment and coating of metals",
      "25620 Machining",
      "25710 Manufacture of cutlery",
      "25720 Manufacture of locks and hinges",
      "25730 Manufacture of tools",
      "25910 Manufacture of steel drums and similar containers",
      "25920 Manufacture of light metal packaging",
      "25930 Manufacture of wire products, chain and springs",
      "25940 Manufacture of fasteners and screw machine products",
      "25990 Manufacture of other fabricated metal products n.e.c.",
      "26110 Manufacture of electronic components",
      "26120 Manufacture of loaded electronic boards",
      "26200 Manufacture of computers and peripheral equipment",
      "26301 Manufacture of telegraph and telephone apparatus and equipment",
      "26309 Manufacture of communication equipment other than telegraph, and telephone apparatus and equipment",
      "26400 Manufacture of consumer electronics",
      "26511 Manufacture of electronic measuring, testing etc. equipment, not for industrial process control",
      "26512 Manufacture of electronic industrial process control equipment",
      "26513 Manufacture of non-electronic measuring, testing etc. equipment, not for industrial process control",
      "26514 Manufacture of non-electronic industrial process control equipment",
      "26520 Manufacture of watches and clocks",
      "26600 Manufacture of irradiation, electromedical and electrotherapeutic equipment",
      "26701 Manufacture of optical precision instruments",
      "26702 Manufacture of photographic and cinematographic equipment",
      "26800 Manufacture of magnetic and optical media",
      "27110 Manufacture of electric motors, generators and transformers",
      "27120 Manufacture of electricity distribution and control apparatus",
      "27200 Manufacture of batteries and accumulators",
      "27310 Manufacture of fibre optic cables",
      "27320 Manufacture of other electronic and electric wires and cables",
      "27330 Manufacture of wiring devices",
      "27400 Manufacture of electric lighting equipment",
      "27510 Manufacture of electric domestic appliances",
      "27520 Manufacture of non-electric domestic appliances",
      "27900 Manufacture of other electrical equipment",
      "28110 Manufacture of engines and turbines, except aircraft, vehicle and cycle engines",
      "28120 Manufacture of fluid power equipment",
      "28131 Manufacture of pumps",
      "28132 Manufacture of compressors",
      "28140 Manufacture of taps and valves",
      "28150 Manufacture of bearings, gears, gearing and driving elements",
      "28210 Manufacture of ovens, furnaces and furnace burners",
      "28220 Manufacture of lifting and handling equipment",
      "28230 Manufacture of office machinery and equipment (except computers and peripheral equipment)",
      "28240 Manufacture of power-driven hand tools",
      "28250 Manufacture of non-domestic cooling and ventilation equipment",
      "28290 Manufacture of other general-purpose machinery n.e.c.",
      "28301 Manufacture of agricultural tractors",
      "28302 Manufacture of agricultural and forestry machinery other than tractors",
      "28410 Manufacture of metal forming machinery",
      "28490 Manufacture of other machine tools",
      "28910 Manufacture of machinery for metallurgy",
      "28921 Manufacture of machinery for mining",
      "28922 Manufacture of earthmoving equipment",
      "28923 Manufacture of equipment for concrete crushing and screening and roadworks",
      "28930 Manufacture of machinery for food, beverage and tobacco processing",
      "28940 Manufacture of machinery for textile, apparel and leather production",
      "28950 Manufacture of machinery for paper and paperboard production",
      "28960 Manufacture of plastics and rubber machinery",
      "28990 Manufacture of other special-purpose machinery n.e.c.",
      "29100 Manufacture of motor vehicles",
      "29201 Manufacture of bodies (coachwork) for motor vehicles (except caravans)",
      "29202 Manufacture of trailers and semi-trailers",
      "29203 Manufacture of caravans",
      "29310 Manufacture of electrical and electronic equipment for motor vehicles and their engines",
      "29320 Manufacture of other parts and accessories for motor vehicles",
      "30110 Building of ships and floating structures",
      "30120 Building of pleasure and sporting boats",
      "30200 Manufacture of railway locomotives and rolling stock",
      "30300 Manufacture of air and spacecraft and related machinery",
      "30400 Manufacture of military fighting vehicles",
      "30910 Manufacture of motorcycles",
      "30920 Manufacture of bicycles and invalid carriages",
      "30990 Manufacture of other transport equipment n.e.c.",
      "31010 Manufacture of office and shop furniture",
      "31020 Manufacture of kitchen furniture",
      "31030 Manufacture of mattresses",
      "31090 Manufacture of other furniture",
      "32110 Striking of coins",
      "32120 Manufacture of jewellery and related articles",
      "32130 Manufacture of imitation jewellery and related articles",
      "32200 Manufacture of musical instruments",
      "32300 Manufacture of sports goods",
      "32401 Manufacture of professional and arcade games and toys",
      "32409 Manufacture of other games and toys, n.e.c.",
      "32500 Manufacture of medical and dental instruments and supplies",
      "32910 Manufacture of brooms and brushes",
      "32990 Other manufacturing n.e.c.",
      "33110 Repair of fabricated metal products",
      "33120 Repair of machinery",
      "33130 Repair of electronic and optical equipment",
      "33140 Repair of electrical equipment",
      "33150 Repair and maintenance of ships and boats",
      "33160 Repair and maintenance of aircraft and spacecraft",
      "33170 Repair and maintenance of other transport equipment n.e.c.",
      "33190 Repair of other equipment",
      "33200 Installation of industrial machinery and equipment",
      "35110 Production of electricity",
      "35120 Transmission of electricity",
      "35130 Distribution of electricity",
      "35140 Trade of electricity",
      "35210 Manufacture of gas",
      "35220 Distribution of gaseous fuels through mains",
      "35230 Trade of gas through mains",
      "35300 Steam and air conditioning supply",
      "36000 Water collection, treatment and supply",
      "37000 Sewerage",
      "38110 Collection of non-hazardous waste",
      "38120 Collection of hazardous waste",
      "38210 Treatment and disposal of non-hazardous waste",
      "38220 Treatment and disposal of hazardous waste",
      "38310 Dismantling of wrecks",
      "38320 Recovery of sorted materials",
      "39000 Remediation activities and other waste management services",
      "41100 Development of building projects",
      "41201 Construction of commercial buildings",
      "41202 Construction of domestic buildings",
      "42110 Construction of roads and motorways",
      "42120 Construction of railways and underground railways",
      "42130 Construction of bridges and tunnels",
      "42210 Construction of utility projects for fluids",
      "42220 Construction of utility projects for electricity and telecommunications",
      "42910 Construction of water projects",
      "42990 Construction of other civil engineering projects n.e.c.",
      "43110 Demolition",
      "43120 Site preparation",
      "43130 Test drilling and boring",
      "43210 Electrical installation",
      "43220 Plumbing, heat and air-conditioning installation",
      "43290 Other construction installation",
      "43310 Plastering",
      "43320 Joinery installation",
      "43330 Floor and wall covering",
      "43341 Painting",
      "43342 Glazing",
      "43390 Other building completion and finishing",
      "43910 Roofing activities",
      "43991 Scaffold erection",
      "43999 Other specialised construction activities n.e.c.",
      "45111 Sale of new cars and light motor vehicles",
      "45112 Sale of used cars and light motor vehicles",
      "45190 Sale of other motor vehicles",
      "45200 Maintenance and repair of motor vehicles",
      "45310 Wholesale trade of motor vehicle parts and accessories",
      "45320 Retail trade of motor vehicle parts and accessories",
      "45400 Sale, maintenance and repair of motorcycles and related parts and accessories",
      "46110 Agents selling agricultural raw materials, livestock, textile raw materials and semi-finished goods",
      "46120 Agents involved in the sale of fuels, ores, metals and industrial chemicals",
      "46130 Agents involved in the sale of timber and building materials",
      "46140 Agents involved in the sale of machinery, industrial equipment, ships and aircraft",
      "46150 Agents involved in the sale of furniture, household goods, hardware and ironmongery",
      "46160 Agents involved in the sale of textiles, clothing, fur, footwear and leather goods",
      "46170 Agents involved in the sale of food, beverages and tobacco",
      "46180 Agents specialised in the sale of other particular products",
      "46190 Agents involved in the sale of a variety of goods",
      "46210 Wholesale of grain, unmanufactured tobacco, seeds and animal feeds",
      "46220 Wholesale of flowers and plants",
      "46230 Wholesale of live animals",
      "46240 Wholesale of hides, skins and leather",
      "46310 Wholesale of fruit and vegetables",
      "46320 Wholesale of meat and meat products",
      "46330 Wholesale of dairy products, eggs and edible oils and fats",
      "46341 Wholesale of fruit and vegetable juices, mineral water and soft drinks",
      "46342 Wholesale of wine, beer, spirits and other alcoholic beverages",
      "46350 Wholesale of tobacco products",
      "46360 Wholesale of sugar and chocolate and sugar confectionery",
      "46370 Wholesale of coffee, tea, cocoa and spices",
      "46380 Wholesale of other food, including fish, crustaceans and molluscs",
      "46390 Non-specialised wholesale of food, beverages and tobacco",
      "46410 Wholesale of textiles",
      "46420 Wholesale of clothing and footwear",
      "46431 Wholesale of audio tapes, records, CDs and video tapes and the equipment on which these are played",
      "46439 Wholesale of radio, television goods & electrical household appliances (other than records, tapes, CD's & video tapes and the equipment used for playing them)",
      "46440 Wholesale of china and glassware and cleaning materials",
      "46450 Wholesale of perfume and cosmetics",
      "46460 Wholesale of pharmaceutical goods",
      "46470 Wholesale of furniture, carpets and lighting equipment",
      "46480 Wholesale of watches and jewellery",
      "46491 Wholesale of musical instruments",
      "46499 Wholesale of household goods (other than musical instruments) n.e.c",
      "46510 Wholesale of computers, computer peripheral equipment and software",
      "46520 Wholesale of electronic and telecommunications equipment and parts",
      "46610 Wholesale of agricultural machinery, equipment and supplies",
      "46620 Wholesale of machine tools",
      "46630 Wholesale of mining, construction and civil engineering machinery",
      "46640 Wholesale of machinery for the textile industry and of sewing and knitting machines",
      "46650 Wholesale of office furniture",
      "46660 Wholesale of other office machinery and equipment",
      "46690 Wholesale of other machinery and equipment",
      "46711 Wholesale of petroleum and petroleum products",
      "46719 Wholesale of other fuels and related products",
      "46720 Wholesale of metals and metal ores",
      "46730 Wholesale of wood, construction materials and sanitary equipment",
      "46740 Wholesale of hardware, plumbing and heating equipment and supplies",
      "46750 Wholesale of chemical products",
      "46760 Wholesale of other intermediate products",
      "46770 Wholesale of waste and scrap",
      "46900 Non-specialised wholesale trade",
      "47110 Retail sale in non-specialised stores with food, beverages or tobacco predominating",
      "47190 Other retail sale in non-specialised stores",
      "47210 Retail sale of fruit and vegetables in specialised stores",
      "47220 Retail sale of meat and meat products in specialised stores",
      "47230 Retail sale of fish, crustaceans and molluscs in specialised stores",
      "47240 Retail sale of bread, cakes, flour confectionery and sugar confectionery in specialised stores",
      "47250 Retail sale of beverages in specialised stores",
      "47260 Retail sale of tobacco products in specialised stores",
      "47290 Other retail sale of food in specialised stores",
      "47300 Retail sale of automotive fuel in specialised stores",
      "47410 Retail sale of computers, peripheral units and software in specialised stores",
      "47421 Retail sale of mobile telephones",
      "47429 Retail sale of telecommunications equipment other than mobile telephones",
      "47430 Retail sale of audio and video equipment in specialised stores",
      "47510 Retail sale of textiles in specialised stores",
      "47520 Retail sale of hardware, paints and glass in specialised stores",
      "47530 Retail sale of carpets, rugs, wall and floor coverings in specialised stores",
      "47540 Retail sale of electrical household appliances in specialised stores",
      "47591 Retail sale of musical instruments and scores",
      "47599 Retail of furniture, lighting, and similar (not musical instruments or scores) in specialised store",
      "47610 Retail sale of books in specialised stores",
      "47620 Retail sale of newspapers and stationery in specialised stores",
      "47630 Retail sale of music and video recordings in specialised stores",
      "47640 Retail sale of sports goods, fishing gear, camping goods, boats and bicycles",
      "47650 Retail sale of games and toys in specialised stores",
      "47710 Retail sale of clothing in specialised stores",
      "47721 Retail sale of footwear in specialised stores",
      "47722 Retail sale of leather goods in specialised stores",
      "47730 Dispensing chemist in specialised stores",
      "47741 Retail sale of hearing aids",
      "47749 Retail sale of medical and orthopaedic goods in specialised stores (not incl. hearing aids) n.e.c.",
      "47750 Retail sale of cosmetic and toilet articles in specialised stores",
      "47760 Retail sale of flowers, plants, seeds, fertilizers, pet animals and pet food in specialised stores",
      "47770 Retail sale of watches and jewellery in specialised stores",
      "47781 Retail sale in commercial art galleries",
      "47782 Retail sale by opticians",
      "47789 Other retail sale of new goods in specialised stores (not commercial art galleries and opticians)",
      "47791 Retail sale of antiques including antique books in stores",
      "47799 Retail sale of other second-hand goods in stores (not incl. antiques)",
      "47810 Retail sale via stalls and markets of food, beverages and tobacco products",
      "47820 Retail sale via stalls and markets of textiles, clothing and footwear",
      "47890 Retail sale via stalls and markets of other goods",
      "47910 Retail sale via mail order houses or via Internet",
      "47990 Other retail sale not in stores, stalls or markets",
      "49100 Passenger rail transport, interurban",
      "49200 Freight rail transport",
      "49311 Urban and suburban passenger railway transportation by underground, metro and similar systems",
      "49319 Other urban, suburban or metropolitan passenger land transport (not underground, metro or similar)",
      "49320 Taxi operation",
      "49390 Other passenger land transport",
      "49410 Freight transport by road",
      "49420 Removal services",
      "49500 Transport via pipeline",
      "50100 Sea and coastal passenger water transport",
      "50200 Sea and coastal freight water transport",
      "50300 Inland passenger water transport",
      "50400 Inland freight water transport",
      "51101 Scheduled passenger air transport",
      "51102 Non-scheduled passenger air transport",
      "51210 Freight air transport",
      "51220 Space transport",
      "52101 Operation of warehousing and storage facilities for water transport activities",
      "52102 Operation of warehousing and storage facilities for air transport activities",
      "52103 Operation of warehousing and storage facilities for land transport activities",
      "52211 Operation of rail freight terminals",
      "52212 Operation of rail passenger facilities at railway stations",
      "52213 Operation of bus and coach passenger facilities at bus and coach stations",
      "52219 Other service activities incidental to land transportation, n.e.c.",
      "52220 Service activities incidental to water transportation",
      "52230 Service activities incidental to air transportation",
      "52241 Cargo handling for water transport activities",
      "52242 Cargo handling for air transport activities",
      "52243 Cargo handling for land transport activities",
      "52290 Other transportation support activities",
      "53100 Postal activities under universal service obligation",
      "53201 Licensed carriers",
      "53202 Unlicensed carriers",
      "55100 Hotels and similar accommodation",
      "55201 Holiday centres and villages",
      "55202 Youth hostels",
      "55209 Other holiday and other collective accommodation",
      "55300 Recreational vehicle parks, trailer parks and camping grounds",
      "55900 Other accommodation",
      "56101 Licenced restaurants",
      "56102 Unlicenced restaurants and cafes",
      "56103 Take-away food shops and mobile food stands",
      "56210 Event catering activities",
      "56290 Other food services",
      "56301 Licenced clubs",
      "56302 Public houses and bars",
      "58110 Book publishing",
      "58120 Publishing of directories and mailing lists",
      "58130 Publishing of newspapers",
      "58141 Publishing of learned journals",
      "58142 Publishing of consumer and business journals and periodicals",
      "58190 Other publishing activities",
      "58210 Publishing of computer games",
      "58290 Other software publishing",
      "59111 Motion picture production activities",
      "59112 Video production activities",
      "59113 Television programme production activities",
      "59120 Motion picture, video and television programme post-production activities",
      "59131 Motion picture distribution activities",
      "59132 Video distribution activities",
      "59133 Television programme distribution activities",
      "59140 Motion picture projection activities",
      "59200 Sound recording and music publishing activities",
      "60100 Radio broadcasting",
      "60200 Television programming and broadcasting activities",
      "61100 Wired telecommunications activities",
      "61200 Wireless telecommunications activities",
      "61300 Satellite telecommunications activities",
      "61900 Other telecommunications activities",
      "62011 Ready-made interactive leisure and entertainment software development",
      "62012 Business and domestic software development",
      "62020 Information technology consultancy activities",
      "62030 Computer facilities management activities",
      "62090 Other information technology service activities",
      "63110 Data processing, hosting and related activities",
      "63120 Web portals",
      "63910 News agency activities",
      "63990 Other information service activities n.e.c.",
      "64110 Central banking",
      "64191 Banks",
      "64192 Building societies",
      "64201 Activities of agricultural holding companies",
      "64202 Activities of production holding companies",
      "64203 Activities of construction holding companies",
      "64204 Activities of distribution holding companies",
      "64205 Activities of financial services holding companies",
      "64209 Activities of other holding companies n.e.c.",
      "64301 Activities of investment trusts",
      "64302 Activities of unit trusts",
      "64303 Activities of venture and development capital companies",
      "64304 Activities of open-ended investment companies",
      "64305 Activities of property unit trusts",
      "64306 Activities of real estate investment trusts",
      "64910 Financial leasing",
      "64921 Credit granting by non-deposit taking finance houses and other specialist consumer credit grantors",
      "64922 Activities of mortgage finance companies",
      "64929 Other credit granting n.e.c.",
      "64991 Security dealing on own account",
      "64992 Factoring",
      "64999 Financial intermediation not elsewhere classified",
      "65110 Life insurance",
      "65120 Non-life insurance",
      "65201 Life reinsurance",
      "65202 Non-life reinsurance",
      "65300 Pension funding",
      "66110 Administration of financial markets",
      "66120 Security and commodity contracts dealing activities",
      "66190 Activities auxiliary to financial intermediation n.e.c.",
      "66210 Risk and damage evaluation",
      "66220 Activities of insurance agents and brokers",
      "66290 Other activities auxiliary to insurance and pension funding",
      "66300 Fund management activities",
      "68100 Buying and selling of own real estate",
      "68201 Renting and operating of Housing Association real estate",
      "68202 Letting and operating of conference and exhibition centres",
      "68209 Other letting and operating of own or leased real estate",
      "68310 Real estate agencies",
      "68320 Management of real estate on a fee or contract basis",
      "69101 Barristers at law",
      "69102 Solicitors",
      "69109 Activities of patent and copyright agents; other legal activities n.e.c.",
      "69201 Accounting and auditing activities",
      "69202 Bookkeeping activities",
      "69203 Tax consultancy",
      "70100 Activities of head offices",
      "70210 Public relations and communications activities",
      "70221 Financial management",
      "70229 Management consultancy activities other than financial management",
      "71111 Architectural activities",
      "71112 Urban planning and landscape architectural activities",
      "71121 Engineering design activities for industrial process and production",
      "71122 Engineering related scientific and technical consulting activities",
      "71129 Other engineering activities",
      "71200 Technical testing and analysis",
      "72110 Research and experimental development on biotechnology",
      "72190 Other research and experimental development on natural sciences and engineering",
      "72200 Research and experimental development on social sciences and humanities",
      "73110 Advertising agencies",
      "73120 Media representation services",
      "73200 Market research and public opinion polling",
      "74100 specialised design activities",
      "74201 Portrait photographic activities",
      "74202 Other specialist photography",
      "74203 Film processing",
      "74209 Photographic activities not elsewhere classified",
      "74300 Translation and interpretation activities",
      "74901 Environmental consulting activities",
      "74902 Quantity surveying activities",
      "74909 Other professional, scientific and technical activities n.e.c.",
      "74990 Non-trading company",
      "75000 Veterinary activities",
      "77110 Renting and leasing of cars and light motor vehicles",
      "77120 Renting and leasing of trucks and other heavy vehicles",
      "77210 Renting and leasing of recreational and sports goods",
      "77220 Renting of video tapes and disks",
      "77291 Renting and leasing of media entertainment equipment",
      "77299 Renting and leasing of other personal and household goods",
      "77310 Renting and leasing of agricultural machinery and equipment",
      "77320 Renting and leasing of construction and civil engineering machinery and equipment",
      "77330 Renting and leasing of office machinery and equipment (including computers)",
      "77341 Renting and leasing of passenger water transport equipment",
      "77342 Renting and leasing of freight water transport equipment",
      "77351 Renting and leasing of air passenger transport equipment",
      "77352 Renting and leasing of freight air transport equipment",
      "77390 Renting and leasing of other machinery, equipment and tangible goods n.e.c.",
      "77400 Leasing of intellectual property and similar products, except copyright works",
      "78101 Motion picture, television and other theatrical casting activities",
      "78109 Other activities of employment placement agencies",
      "78200 Temporary employment agency activities",
      "78300 Human resources provision and management of human resources functions",
      "79110 Travel agency activities",
      "79120 Tour operator activities",
      "79901 Activities of tourist guides",
      "79909 Other reservation service activities n.e.c.",
      "80100 Private security activities",
      "80200 Security systems service activities",
      "80300 Investigation activities",
      "81100 Combined facilities support activities",
      "81210 General cleaning of buildings",
      "81221 Window cleaning services",
      "81222 Specialised cleaning services",
      "81223 Furnace and chimney cleaning services",
      "81229 Other building and industrial cleaning activities",
      "81291 Disinfecting and exterminating services",
      "81299 Other cleaning services",
      "81300 Landscape service activities",
      "82110 Combined office administrative service activities",
      "82190 Photocopying, document preparation and other specialised office support activities",
      "82200 Activities of call centres",
      "82301 Activities of exhibition and fair organisers",
      "82302 Activities of conference organisers",
      "82911 Activities of collection agencies",
      "82912 Activities of credit bureaus",
      "82920 Packaging activities",
      "82990 Other business support service activities n.e.c.",
      "84110 General public administration activities",
      "84120 Regulation of health care, education, cultural and other social services, not incl. social security",
      "84130 Regulation of and contribution to more efficient operation of businesses",
      "84210 Foreign affairs",
      "84220 Defence activities",
      "84230 Justice and judicial activities",
      "84240 Public order and safety activities",
      "84250 Fire service activities",
      "84300 Compulsory social security activities",
      "85100 Pre-primary education",
      "85200 Primary education",
      "85310 General secondary education",
      "85320 Technical and vocational secondary education",
      "85410 Post-secondary non-tertiary education",
      "85421 First-degree level higher education",
      "85422 Post-graduate level higher education",
      "85510 Sports and recreation education",
      "85520 Cultural education",
      "85530 Driving school activities",
      "85590 Other education n.e.c.",
      "85600 Educational support services",
      "86101 Hospital activities",
      "86102 Medical nursing home activities",
      "86210 General medical practice activities",
      "86220 Specialists medical practice activities",
      "86230 Dental practice activities",
      "86900 Other human health activities",
      "87100 Residential nursing care facilities",
      "87200 Residential care activities for learning difficulties, mental health and substance abuse",
      "87300 Residential care activities for the elderly and disabled",
      "87900 Other residential care activities n.e.c.",
      "88100 Social work activities without accommodation for the elderly and disabled",
      "88910 Child day-care activities",
      "88990 Other social work activities without accommodation n.e.c.",
      "90010 Performing arts",
      "90020 Support activities to performing arts",
      "90030 Artistic creation",
      "90040 Operation of arts facilities",
      "91011 Library activities",
      "91012 Archives activities",
      "91020 Museums activities",
      "91030 Operation of historical sites and buildings and similar visitor attractions",
      "91040 Botanical and zoological gardens and nature reserves activities",
      "92000 Gambling and betting activities",
      "93110 Operation of sports facilities",
      "93120 Activities of sport clubs",
      "93130 Fitness facilities",
      "93191 Activities of racehorse owners",
      "93199 Other sports activities",
      "93210 Activities of amusement parks and theme parks",
      "93290 Other amusement and recreation activities n.e.c.",
      "94110 Activities of business and employers membership organisations",
      "94120 Activities of professional membership organisations",
      "94200 Activities of trade unions",
      "94910 Activities of religious organisations",
      "94920 Activities of political organisations",
      "94990 Activities of other membership organisations n.e.c.",
      "95110 Repair of computers and peripheral equipment",
      "95120 Repair of communication equipment",
      "95210 Repair of consumer electronics",
      "95220 Repair of household appliances and home and garden equipment",
      "95230 Repair of footwear and leather goods",
      "95240 Repair of furniture and home furnishings",
      "95250 Repair of watches, clocks and jewellery",
      "95290 Repair of personal and household goods n.e.c.",
      "96010 Washing and (dry-)cleaning of textile and fur products",
      "96020 Hairdressing and other beauty treatment",
      "96030 Funeral and related activities",
      "96040 Physical well-being activities",
      "96090 Other service activities n.e.c.",
      "97000 Activities of households as employers of domestic personnel",
      "98000 Residents property management",
      "98100 Undifferentiated goods-producing activities of private households for own use",
      "98200 Undifferentiated service-producing activities of private households for own use",
      "99000 Activities of extraterritorial organisations and bodies",
      "99999 Dormant Company",
    ];
  }

  static getNatureOfBusinessBySicCode(
    sicCode: string | undefined
  ): string | undefined {
    if (!sicCode) {
      return undefined;
    }

    const natureOfBusinesses = this.getNatureOfBusinesses();
    return natureOfBusinesses.find((value) => value.startsWith(sicCode));
  }

  static getProductTypes(name: string): string[] {
    switch (name) {
      case "Mortgage Broking":
      case "Mortgage Lending":
        return [
          "Owner Occupier Mortgages",
          "Owner Occupier Second Charge",
          "Owner Occupier Bridging Loans",
          "Consumer Buy to Let Mortgages",
          "Consumer Buy to Let Second Charge",
          "Consumer Buy to Let Bridging Loans",
          "Equity Release",
          "Professional Buy to Let Mortgages",
          "Professional Buy to Let Second Charge",
          "Professional Buy to Let Bridging Loans",
          "Commercial Mortgages",
          "Commercial Second Charge",
          "Commercial Bridging Loans",
        ];
      case "Protection Broking":
        return [
          "Life - Fixed Term",
          "Life - Whole of Life",
          "Family Income Benefit",
          "Critical Illness Cover",
          "Income Protection Insurance",
          "Personal Medical Insurance",
          "Relevant Life",
          "Key Person",
          "Shareholder Protection",
        ];
      case "General Insurance Broking":
      case "General Insurance Underwriting":
        return [
          "Buildings Insurance",
          "Contents Insurance",
          "Landlords Insurance",
          "Commercial Property Insurance",
          "Professional Indemnity Insurance",
          "Employer Liability Insurance",
          "Public Liability Insurance",
          "Business Interruption Insurance",
          "Directors and Officers Liability",
          "Motor Insurance (Private Use)",
          "Motor Insurance (Business Use)",
          "Motor Insurance (Fleet)",
          "Pet Insurance",
        ];
      case "Consumer Credit":
        return [
          "Credit Broking",
          "Debt Counselling",
          "Debt Adjusting",
          "Debt Collecting",
          "Lending of Regulated Unsecured Loans ",
          "Lending of Non-Regulated Unsecured Loans",
        ];
      case "Investments":
        return ["Readily Realisable Securities", "Cryptoassets", "Pensions"];
      case "Banking":
        return [
          "Deposit Accounts (Current and Savings)",
          "Payment Services (excl. E-Money)",
          "Payment Services (incl. E-Money)",
        ];
      default:
        return [];
    }
  }

  static getRemunerationMethods(): string[] {
    return [
      "Per Account Opening",
      "Per Lead Generated",
      "Per Click",
      "Per Post Added",
      "Fixed Fee",
      "No Remuneration",
    ];
  }

  static getProducts(): string[] {
    return [
      "Mortgage Broking",
      "Protection Broking",
      "General Insurance Broking",
      "Consumer Credit",
    ];
  }

  static getProductsAndServices(): string[] {
    return [
      "Mortgage Broking",
      "Protection Broking",
      "General Insurance Broking",
      "Consumer Credit",
      "Other",
    ];
  }

  static getRefuseStatuses(): string[] {
    return ["Removed", "Refused"];
  }

  static getStatuses(): string[] {
    return ["Active", "Onboarding", "Suspended", "Terminated"];
  }

  static getOrgStructureRoles(): string[] {
    const roles = this.getRoles();
    roles.splice(roles.length - 1, 1);
    roles.push("General Insurance Broker");
    return roles;
  }

  static getPostApprovalTypes(): string[] {
    return ["New Post", "Existing Post", "Reinstated Post"];
  }

  static getARProductTypes(): ArProductTypesModel[] {
    return [
      {
        id: "mortgage-broking",
        title: "Mortgage Broking",
      },
      {
        id: "protection-broking",
        title: "Protection Broking",
      },
      {
        id: "general-insurance-broking",
        title: "General Insurance Broking",
      },
      {
        id: "consumer-credit",
        title: "Consumer Credit",
      },
    ];
  }

  static getARProducts(): ArProductsModel[] {
    return [
      {
        id: "owner-occupier-mortgages",
        typeId: "mortgage-broking",
        title: "Owner Occupier Mortgages",
      },
      {
        id: "owner-occupier-second-charge",
        typeId: "mortgage-broking",
        title: "Owner Occupier Second Charge",
      },
      {
        id: "owner-occupier-bridging-loans",
        typeId: "mortgage-broking",
        title: "Owner Occupier Bridging Loans",
      },
      {
        id: "equity-release",
        typeId: "mortgage-broking",
        title: "Equity Release",
      },
      {
        id: "consumer-buy-to-let-mortgages",
        typeId: "mortgage-broking",
        title: "Consumer Buy to Let Mortgages",
      },
      {
        id: "consumer-buy-to-let-second-charge",
        typeId: "mortgage-broking",
        title: "Consumer Buy to Let Second Charge",
      },
      {
        id: "consumer-buy-to-let-bridging-loans",
        typeId: "mortgage-broking",
        title: "Consumer Buy to Let Bridging Loans",
      },
      {
        id: "professional-buy-to-let-mortgages",
        typeId: "mortgage-broking",
        title: "Professional Buy to Let Mortgages",
      },
      {
        id: "professional-buy-to-let-second-charge",
        typeId: "mortgage-broking",
        title: "Professional Buy to Let Second Charge",
      },
      {
        id: "professional-buy-to-let-bridging-loans",
        typeId: "mortgage-broking",
        title: "Professional Buy to Let Bridging Loans",
      },
      {
        id: "commercial-mortgages",
        typeId: "mortgage-broking",
        title: "Commercial Mortgages",
      },
      {
        id: "commercial-second-charge",
        typeId: "mortgage-broking",
        title: "Commercial Second Charge",
      },
      {
        id: "commercial-bridging-loans",
        typeId: "mortgage-broking",
        title: "Commercial Bridging Loans",
      },
      {
        id: "life-insurance-fixed-term",
        typeId: "protection-broking",
        title: "Life Insurance - Fixed Term",
      },
      {
        id: "life-insurance-whole-of-life",
        typeId: "protection-broking",
        title: "Life Insurance - Whole of Life",
      },
      {
        id: "critical-illness-cover",
        typeId: "protection-broking",
        title: "Critical Illness Cover",
      },
      {
        id: "income-protection",
        typeId: "protection-broking",
        title: "Income Protection",
      },
      {
        id: "key-person",
        typeId: "protection-broking",
        title: "Key Person",
      },
      {
        id: "relevant-life",
        typeId: "protection-broking",
        title: "Relevant Life",
      },
      {
        id: "shareholder-protection",
        typeId: "protection-broking",
        title: "Shareholder Protection",
      },
      {
        id: "private-mecial-insurance",
        typeId: "protection-broking",
        title: "Private Mecial Insurance",
      },
      {
        id: "buildings-and-contents",
        typeId: "general-insurance-broking",
        title: "Buildings and Contents",
      },
      {
        id: "landlords-insurance",
        typeId: "general-insurance-broking",
        title: "Landlords Insurance",
      },
      {
        id: "commercial-property",
        typeId: "general-insurance-broking",
        title: "Commercial Property",
      },
      {
        id: "business-interruption",
        typeId: "general-insurance-broking",
        title: "Business Interruption",
      },
      {
        id: "professional-indemnity",
        typeId: "general-insurance-broking",
        title: "Professional Indemnity",
      },
      {
        id: "employers-liability",
        typeId: "general-insurance-broking",
        title: "Employers Liability",
      },
      {
        id: "public-liability",
        typeId: "general-insurance-broking",
        title: "Public Liability",
      },
      {
        id: "directors-and-officers-liability",
        typeId: "general-insurance-broking",
        title: "Directors and Officers Liability",
      },
      {
        id: "credit-broking",
        typeId: "consumer-credit",
        title: "Credit Broking",
      },
      {
        id: "debt-counselling",
        typeId: "consumer-credit",
        title: "Debt Counselling",
      },
      {
        id: "debt-adjusting",
        typeId: "consumer-credit",
        title: "Debt Adjusting",
      },
      {
        id: "debt-collection",
        typeId: "consumer-credit",
        title: "Debt Collection",
      },
      {
        id: "readily-realisable-securities",
        typeId: "investments",
        title: "Readily Realisable Securities",
      },
      {
        id: "certificates-representing-certain-securities",
        typeId: "investments",
        title: "Certificates Representing Certain Securities",
      },
      {
        id: "government-and-public-security",
        typeId: "investments",
        title: "Government and Public Security",
      },
      {
        id: "rights-to-or-interests-in-investments",
        typeId: "investments",
        title: "Rights to or Interests in Investments",
      },
      {
        id: "units",
        typeId: "investments",
        title: "Units",
      },
      {
        id: "options",
        typeId: "investments",
        title: "Options",
      },
      {
        id: "futures",
        typeId: "investments",
        title: "Futures",
      },
      {
        id: "cryptoassets",
        typeId: "investments",
        title: "Cryptoassets",
      },
      {
        id: "pensions",
        typeId: "investments",
        title: "Pensions",
      },
      {
        id: "other",
        typeId: "investments",
        title: "Other",
      },
      {
        id: "deposit-accounts",
        typeId: "banking",
        title: "Deposit Accounts (Current and Savings)",
      },
      {
        id: "payment-services-excluding-e-money",
        typeId: "banking",
        title: "Payment Services (excl. E-Money)",
      },
      {
        id: "payment-services-including-e-money",
        typeId: "banking",
        title: "Payment Services (incl. E-Money)",
      },
    ];
  }

  static getARStatuses(): ArStatusesModel[] {
    return [
      {
        label: "Active",
        value: 0,
      },
      {
        label: "Onboarding",
        value: 1,
      },
      {
        label: "Suspended",
        value: 2,
      },
      {
        label: "Terminated",
        value: 3,
      },
    ];
  }

  static getARMarketService(): ArMarketServiceModel[] {
    return [
      {
        market: "Home Finance Mediation Activity",
        subMarket: [
          "Home Purchase Mediation Activity",
          "Mortgage Mediation Activity",
          "Reversion Mediation Activity",
        ],
      },
      {
        market: "Credit Related Regulated Activity",
        subMarket: [
          "Credit Broking",
          "Operating an Electronic System in Relation to Lending",
          "Other Credit-Related Activity",
        ],
      },
      {
        market: "Insurance Distribution Activity",
        subMarket: [],
      },
    ];
  }

  static getARActivityProducts(): ArActivityProductsModel[] {
    return [
      {
        id: "mortgage-broking",
        title: "Mortgage Broking",
        icon: "share-money-dollar-22",
      },
      {
        id: "protection-broking",
        title: "Protection Broking",
        icon: "user-protection-2â__shield-secure-security-profile-person-3",
      },
      {
        id: "general-insurance-broking",
        title: "General Insurance Broking",
        icon: "shield-check-4",
      },
      {
        id: "consumer-credit",
        title: "Consumer Credit",
        icon: "user-protection-check",
      },
      {
        id: "investments",
        title: "Investments",
        icon: "investment-selection-41",
      },
      {
        id: "banking",
        title: "Banking",
        icon: "bank",
      },
    ];
  }

  static getRegulatedActivities(): string[] {
    return [
      "Home Finance Mediation Activity",
      "Credit Related Regulated Activity",
      "Insurance Distribution Activity",
      "Other",
    ];
  }

  static getTerminationReasons(): string[] {
    return [
      "End of Contract",
      "Terminated by Principal",
      "Retirement",
      "Terminated by Appointed Representative",
    ];
  }

  static getEmploymentStatuses(): string[] {
    return ["Active", "Onboarding", "Suspended", "Terminated"];
  }

  static getDefaultDisclosure(): DisclosureEntity {
    return {
      timePeriodDisclosureText: [
        {
          content: "<p>Accurate at the point of publication.</p>",
        },
      ],
      timePeriodDisclosureConfirmedText: [],
      affiliateDisclosureText: [
        {
          content:
            "<p>Some of the products featured are from affiliate partners from whom we receive compensation.</p><p>Whilst we aim to feature some of the best products available, this does not include all available products from across the market. Although the information provided is believed to be accurate at the date of publication, you should always check with the product provider to ensure that information provided is the most up to date.</p>",
        },
      ],
      affiliateDisclosureConfirmedText: [],
      taxDisclosureText: [
        {
          content:
            "<p>Tax treatment depends on one’s individual circumstances and may be subject to future change.</p><p>The content of this article is provided for information purposes only and is not intended to be, nor does it constitute, any form of tax advice.</p>",
        },
      ],
      taxDisclosureConfirmedText: [],
      mortgageDisclosureText: [
        {
          content:
            "<p>Think carefully before securing any debts against your home. Your home or property may be repossessed if you do not keep up repayments on a mortgage or any other debt secured on it.</p>",
        },
      ],
      mortgageDisclosureConfirmedText: [],
      investmentDisclosureText: [
        {
          content:
            "<p>Capital at risk. All investments carry a varying degree of risk and it’s important you understand the nature of these. The value of your investments can go down as well as up and you may get back less than you put in. </p>",
        },
        {
          content:
            "<p>Where we feature an investment product or service from an affiliate partner, our promotion is limited to that of their listed stocks & shares platform. We do not promote or encourage any other products such as contract for difference, spread betting or forex.</p><p>Investments in a currency other than sterling are exposed to currency exchange risk. Currency exchange rates are constantly changing which may affect the value of the investment in sterling terms. You could lose money in sterling even if the stock price rises in the currency of origin. Stocks listed on overseas exchanges may be subject to additional dealing and exchange rate charges, and may have other tax implications, and may not provide the same, or any, regulatory protection as in the UK.</p>",
        },
      ],
      investmentDisclosureConfirmedText: [],
      cryptoDisclosureText: [
        {
          content:
            "<p>Don’t invest unless you’re prepared to lose all the money you invest. This is a high‑risk investment and you should not expect to be protected if something goes wrong.<a href='#'>Take 2 mins to learn more</a>.</p><p><strong>Cryptocurrency Disclosure</strong></p><strong>Estimated reading time: 2 min</strong><br><p>Due to the potential for losses, the Financial Conduct Authority (FCA) considers this investment to be high risk.</p><strong>What are the key risks?</strong><br><ol><li><strong>You could lose all the money you invest.</strong><br><ul><li>The performance of most cryptoassets can be highly volatile, with their value dropping as quickly as it can rise. You should be prepared to lose all the money you invest in cryptoassets.</li><li>The cryptoasset market is generally unregulated. There is a risk of losing money or any cryptoassets you purchase due to risks such as cyber-attacks, financial crime and firm failure.</li></ul></li><li><strong>You should not expect to be protected if something goes wrong.</strong><br><ul><li>The Financial Services Compensation Scheme (FSCS) doesn’t protect this type of investment because it’s not a ‘specified investment’ under the UK regulatory regime – in other words, this type of investment isn’t recognised as the sort of investment that the FSCS can protect. Learn more by using the FSCS investment protection checker here.</li><li>Protection from the Financial Ombudsman Service (FOS) does not cover poor investment performance. If you have a complaint against an FCA regulated firm, FOS may be able to consider it. Learn more about FOS protection here.</li></ul></li><li><strong>You may not be able to sell your investment when you want to.</strong><br><ul><li>There is no guarantee that investments in cryptoassets can be easily sold at any given time. The ability to sell a cryptoasset depends on various factors, including the supply and demand in the market at that time.</li><li>Operational failings such as technology outages, cyber-attacks and comingling of funds could cause unwanted delay and you may be unable to sell your cryptoassets at the time you want.</li></ul></li><li><strong>Cryptoasset investments can be complex.</strong><br><ul><li>Investments in cryptoassets can be complex, making it difficult to understand the risks associated with the investment.</li><li>You should do your own research before investing. If something sounds too good to be true, it probably is.</li></ul></li><li><strong>Don’t put all your eggs in one basket.</strong><br><ul><li>Putting all your money into a single type of investment is risky. Spreading your money across different investments makes you less dependent on any one to do well.</li><li>A good rule of thumb is not to invest more than 10% of your money in <a href='https://www.fca.org.uk/investsmart/5-questions-ask-you-invest' target='_blank'>high-risk investments</a>.</li></ul></li></ol><p><strong>If you are interested in learning more about how to protect yourself, visit the FCA’s website hereIf you are interested in learning more about how to protect yourself, visit the FCA’s website here</strong></p><p><strong>For further information about cryptoassets, visit the FCA’s website <a href='https://www.fca.org.uk/investsmart/crypto-basics' target='blank' class='text-primary'>here</a></strong></p>",
        },
      ],
      cryptoDisclosureConfirmedText: [],
    } as DisclosureEntity;
  }

  static getDefaultStationeries(): StationeryEntity[] {
    const dateNow = moment().unix() as number;

    return [
      {
        id: uuid(),
        name: "Logo",
        icon: "pictures-folder-memories-25",
        files: [],
        status: StationeryStatus.Approved,
        createdAt: dateNow,
        updatedAt: dateNow,
      },
      {
        id: uuid(),
        name: "Business Card",
        icon: "business-card",
        files: [],
        status: StationeryStatus.Approved,
        createdAt: dateNow,
        updatedAt: dateNow,
      },
      {
        id: uuid(),
        name: "Letterhead",
        icon: "image-caption-39",
        files: [],
        status: StationeryStatus.Approved,
        createdAt: dateNow,
        updatedAt: dateNow,
      },
      {
        id: uuid(),
        name: "Compliment Slip",
        icon: "text-file-17",
        files: [],
        status: StationeryStatus.Approved,
        createdAt: dateNow,
        updatedAt: dateNow,
      },
      {
        id: uuid(),
        name: "Email Footer",
        icon: "form-email-71",
        files: [],
        status: StationeryStatus.Approved,
        createdAt: dateNow,
        updatedAt: dateNow,
      },
    ] as StationeryEntity[];
  }

  static getDefaultDocumentFormattings(): DocumentFormatting[] {
    const dateNow = moment().unix() as number;

    return [
      {
        id: "title-1",
        name: "Title 1",
        font: "Inter, sans-serif",
        size: 16,
        isBold: false,
        isItalic: false,
        isUnderline: false,
        alignment: "left",
        textCase: "",
        updatedAt: dateNow,
      },
      {
        id: "title-2",
        name: "Title 2",
        font: "Inter, sans-serif",
        size: 14,
        isBold: false,
        isItalic: false,
        isUnderline: false,
        alignment: "left",
        textCase: "",
        updatedAt: dateNow,
      },
      {
        id: "title-3",
        name: "Header 1",
        font: "Figtree, sans-serif",
        size: 16,
        isBold: false,
        isItalic: false,
        isUnderline: false,
        alignment: "left",
        textCase: "",
        updatedAt: dateNow,
      },
      {
        id: "header-2",
        name: "Header 2",
        font: "Figtree, sans-serif",
        size: 14,
        isBold: false,
        isItalic: false,
        isUnderline: false,
        alignment: "left",
        textCase: "",
        updatedAt: dateNow,
      },
      {
        id: "",
        name: "Paragraph",
        font: "Figtree, sans-serif",
        size: 12,
        isBold: false,
        isItalic: false,
        isUnderline: false,
        alignment: "left",
        textCase: "",
        updatedAt: dateNow,
      },
    ] as DocumentFormatting[];
  }

  static getMediaPlatforms(): MediaPlatform[] {
    return  [
      {
        id: "facebook",
        name: "Facebook",
        icon: "media-facebook.svg",
      },
      {
        id: "facebook-ads",
        name: "Facebook",
        icon: "media-facebook.svg",
      },
      {
        id: "google-ads",
        name: "Google",
        icon: "media-google_ads.svg",
      },
      {
        id: "instagram",
        name: "Instagram",
        icon: "media-instagram.svg",
      },
      {
        id: "instagram-ads",
        name: "Instagram",
        icon: "media-instagram.svg",
      },
      {
        id: "linkedin",
        name: "LinkedIn",
        icon: "media-linkedin.svg",
      },
      {
        id: "linkedin-ads",
        name: "LinkedIn",
        icon: "media-linkedin.svg",
      },
      {
        id: "twitter",
        name: "Twitter",
        icon: "media-twitter.svg",
      },
      {
        id: "twitter-ads",
        name: "Twitter",
        icon: "media-twitter.svg",
      },
      {
        id: "website",
        name: "Website",
        icon: "media-website.svg",
      },
    ] as MediaPlatform[];
  }
}
