const KENDO_VUE_BUTTONS = "@progress/kendo-vue-buttons";
const KENDO_VUE_DIALOGS = "@progress/kendo-vue-dialogs";
const KENDO_VUE_DROPDOWNS = "@progress/kendo-vue-dropdowns";
const KENDO_VUE_EXCEL_EXPORT = "@progress/kendo-vue-excel-export";
const KENDO_VUE_DATE_INPUTS = "@progress/kendo-vue-dateinputs";
const KENDO_VUE_FORM = "@progress/kendo-vue-form";
const KENDO_VUE_GRID = "@progress/kendo-vue-grid";
const KENDO_VUE_INPUTS = "@progress/kendo-vue-inputs";
const KENDO_VUE_LABELS = "@progress/kendo-vue-labels";
const KENDO_VUE_LAYOUT = "@progress/kendo-vue-layout";
const KENDO_VUE_PDF = "@progress/kendo-vue-pdf";
const KENDO_VUE_ANIMATION = "@progress/kendo-vue-animation";
const KENDO_VUE_INDICATORS = "@progress/kendo-vue-indicators";
const KENDO_VUE_LISTBOX = "@progress/kendo-vue-listbox";
const KENDO_VUE_LISTVIEW = "@progress/kendo-vue-listview";
const KENDO_VUE_POPUP = "@progress/kendo-vue-popup";
const KENDO_VUE_UPLOAD = "@progress/kendo-vue-upload";
const KENDO_VUE_NOTIFICATION = "@progress/kendo-vue-notification";
const KENDO_VUE_EDITOR = "@progress/kendo-vue-editor";
const KENDO_VUE_WRAPPER = "@progress/kendo-layout-vue-wrapper";
const KENDO_VUE_PROGRESSBARS = "@progress/kendo-vue-progressbars";
const KENDO_VUE_TOOLTIP = "@progress/kendo-vue-tooltip";
const KENDO_VUE_SCROLLVIEW = "@progress/kendo-vue-scrollview";

// @TODO List down all kendo components
const COMPONENTS = {
  AppBar: KENDO_VUE_LAYOUT,
  AppBarSection: KENDO_VUE_LAYOUT,
  AppBarSpacer: KENDO_VUE_LAYOUT,
  Avatar: KENDO_VUE_LAYOUT,
  Button: KENDO_VUE_BUTTONS,
  ButtonGroup: KENDO_VUE_BUTTONS,
  Card: KENDO_VUE_LAYOUT,
  CardActions: KENDO_VUE_LAYOUT,
  CardBody: KENDO_VUE_LAYOUT,
  CardFooter: KENDO_VUE_LAYOUT,
  CardHeader: KENDO_VUE_LAYOUT,
  CardImage: KENDO_VUE_LAYOUT,
  CardSubtitle: KENDO_VUE_LAYOUT,
  CardTitle: KENDO_VUE_LAYOUT,
  ComboBox: KENDO_VUE_DROPDOWNS,
  ContextMenu: KENDO_VUE_WRAPPER,
  Checkbox: KENDO_VUE_INPUTS,
  ChunkProgressBar: KENDO_VUE_PROGRESSBARS,
  DatePicker: KENDO_VUE_DATE_INPUTS,
  Dialog: KENDO_VUE_DIALOGS,
  DialogActionsBar: KENDO_VUE_DIALOGS,
  Drawer: KENDO_VUE_LAYOUT,
  DrawerContent: KENDO_VUE_LAYOUT,
  DrawerItem: KENDO_VUE_LAYOUT,
  DropDownList: KENDO_VUE_DROPDOWNS,
  Editor: KENDO_VUE_EDITOR,
  Error: KENDO_VUE_LABELS,
  ExpansionPanel: KENDO_VUE_LAYOUT,
  ExpansionPanelContent: KENDO_VUE_LAYOUT,
  Fade: KENDO_VUE_ANIMATION,
  Field: KENDO_VUE_FORM,
  FieldArray: KENDO_VUE_FORM,
  FieldWrapper: KENDO_VUE_FORM,
  Form: KENDO_VUE_FORM,
  FormElement: KENDO_VUE_FORM,
  Grid: KENDO_VUE_GRID,
  GridPdfExport: KENDO_VUE_PDF,
  GridToolbar: KENDO_VUE_GRID,
  Hint: KENDO_VUE_LABELS,
  Input: KENDO_VUE_INPUTS,
  Label: KENDO_VUE_LABELS,
  ListBox: KENDO_VUE_LISTBOX,
  ListBoxToolbar: KENDO_VUE_LISTBOX,
  ListView: KENDO_VUE_LISTVIEW,
  ListViewHeader: KENDO_VUE_LISTVIEW,
  Loader: KENDO_VUE_INDICATORS,
  MaskedTextBox: KENDO_VUE_INPUTS,
  Menu: KENDO_VUE_LAYOUT,
  MultiSelectTree: KENDO_VUE_DROPDOWNS,
  Notification: KENDO_VUE_NOTIFICATION,
  NotificationGroup: KENDO_VUE_NOTIFICATION,
  NumericTextBox: KENDO_VUE_INPUTS,
  Popup: KENDO_VUE_POPUP,
  ProgressBar: KENDO_VUE_PROGRESSBARS,
  Reveal: KENDO_VUE_ANIMATION,
  SaveExcel: KENDO_VUE_EXCEL_EXPORT,
  ScrollView: KENDO_VUE_SCROLLVIEW,
  Skeleton: KENDO_VUE_INDICATORS,
  Slider: KENDO_VUE_INPUTS,
  SliderLabel: KENDO_VUE_INPUTS,
  StackLayout: KENDO_VUE_LAYOUT,
  Switch: KENDO_VUE_INPUTS,
  TabStrip: KENDO_VUE_LAYOUT,
  TextArea: KENDO_VUE_INPUTS,
  ToggleButton: KENDO_VUE_DATE_INPUTS,
  Upload: KENDO_VUE_UPLOAD,
  UploadListSingleItem: KENDO_VUE_UPLOAD,
  AutoComplete: KENDO_VUE_DROPDOWNS,
  Tooltip: KENDO_VUE_TOOLTIP,
  MultiSelect: KENDO_VUE_DROPDOWNS,
  RadioGroup: KENDO_VUE_INPUTS,
};

export default (name) => {
  name = name.replace(/^Kendo/, "");
  if (COMPONENTS[name]) {
    return { name, from: COMPONENTS[name] };
  }
};
