import{j as T,l as D,D as $,I as L,A as p,bj as V,bk as B,bl as f,a3 as S,_ as z,o as n,m as l,n as i,w as r,bg as E,bm as G,r as c,x as o,P as d,y as q,J as b,K as U,bn as K,bi as M,bo as O,bp as j,N as H,O as J,bq as W,a2 as Q,bs as X,br as Y}from"./index-9d6d511f.js";import{_ as Z,a as x}from"./rejected-grey-icon-2a930ba1.js";const ee=T({name:"ProvidersList",props:{providers:{type:Array,default:()=>[]}},data(){return{hasProviders:!1,columnMenu:!0,selectedField:"selected",expandField:"expanded",gridPageable:{buttonCount:5,info:!0,type:"numeric",pageSizes:!0,previousNext:!0},gridData:[],tasks:[],skip:0,take:7,group:[],sortParams:[],filter:null,isShowTasksModal:!1,fcaFirmRefNo:"",providerTableGroups:[{title:"Current",value:"current"},{title:"All",value:"all"}],activeProviderTableGroup:"current",providersInternal:[],selectedProducts:[],selectedStatus:{label:"",value:""},eventBus:D("$eventBusService"),helperService:$.resolve(L.name),isInitializing:!0,itemsToPDF:[]}},mounted(){this.providersInternal=this.providers,this.isInitializing=!1},unmounted(){this.eventBus.off(p.filterGridEvent)},computed:{ProviderRepresentative(){return V},ProviderIntroducerDetails(){return B},providersColumns(){return[{field:"details.name",title:"Provider Name",width:"200",resizable:!0},{field:"details.fcaFirmRefNo",title:"FRN",width:70,resizable:!0},{field:"praAuthorised",title:"PRA Authorised",resizable:!0},{field:"products",title:"Products",resizable:!0},{field:"introducer",title:"Provider Representative",resizable:!0},{field:"status",title:"Status"},{field:"tasks",title:"Tasks",width:70},{field:"actions",title:" ",className:"text-center"}]},columnsToPDF(){return this.providersColumns.filter(e=>e.field!=="actions")}},created(){const e=f(this.gridData,this.sortParams);this.$emit("exportingPDF",this.columnsToPDF,e),this.eventBus.on(p.filterGridEvent,()=>{this.skip!=0&&(this.skip=0)})},methods:{orderBy:f,closeForm(){this.isShowTasksModal=!1},emitAddNewProvider(){this.$emit("addNewProvider")},emitViewProviderFromList(e){this.$emit("viewProviderList",e)},emitEditProviderList(e){this.$emit("editProviderList",e)},emitViewTasksFromCard(e,s){this.$emit("viewTasksCard",e,s)},emitViewProviderFromCard(e){this.$emit("viewProviderCard",e)},emitProviderFromCard(e){this.$emit("editProviderCard",e)},emitFilterProvidersByProductsStatus(){this.providersInternal=this.providers.filter(e=>{const s=e.products.map(m=>m.name),a=this.selectedProducts.every(m=>s.includes(m)),u=e.status===this.selectedStatus.value;return a&&u})},emitSearchProvidersByNameFRN(e){if(!e){this.providersInternal=this.providers;return}this.providersInternal=this.providers.filter(s=>{if(!s.details.name||!s.details.fcaFirmRefNo)return!1;const a=s.details.name.toLowerCase().includes(e.toLowerCase()),u=s.details.fcaFirmRefNo.toLowerCase().includes(e.toLowerCase());return a||u})},emitClearFilters(){this.selectedProducts=[],this.selectedStatus={label:"",value:""}},emitCloseFilter(){this.providersInternal=this.providers,this.emitClearFilters()},exportToPdf(e){const s=this.formatProvidersPdfData(e);this.itemsToPDF=f(s,this.sortParams),this.$refs.gridPdfExport.save(this.itemsToPDF)},formatProvidersPdfData(e){return e.map(s=>{var a;return{...s,"details.name":`${(a=s.details)==null?void 0:a.name} `,"details.fcaFirmRefNo":s.details.fcaFirmRefNo,praAuthorised:s.details.praAuthorised,products:s.products,introducer:`${s.representative.forename} ${s.representative.surname}`,status:s.status,tasks:s.tasks}})},getProducts(){return S.getProducts()},getStatuses(){return S.getStatuses().map(e=>({label:e,value:e}))},handleStatusChange(e){this.selectedStatus=e},handleSort(e){this.sortParams=e.sort},handlePage(e){this.skip=e.page.skip,this.take=e.page.take},setUniqueIdentifier(e){const s=`${p.arProvidersRoute}-list${e}`;return this.helperService.removeStringSpacesThenSlash(s)}}});const te={class:"fca-firm-ref-no-cell"},se=["href"],re={key:0,src:Z,alt:"Authorised",title:"Authorised",width:"16",height:"16"},ie={key:1,src:x,alt:"Non-Authorised",title:"Non-Authorised",width:"16",height:"16"},oe={style:{"text-decoration":"underline"}},ae={class:"text-center"},ne={class:"row-name"},le={class:"d-inline-flex gap-2"},de={class:"row mt-2"};function ue(e,s,a,u,m,me){var C;const k=K,N=M,P=O,w=j,v=H,g=J,I=W,y=Q,R=X,A=Y;return n(),l(b,null,[i(N,{ref:"gridPdfExport"},{default:r(()=>[E(i(k,{"data-items":e.itemsToPDF,columns:e.providersColumns},null,8,["data-items","columns"]),[[G,!1]])]),_:1},512),i(A,{isSortable:"",columns:e.providersColumns,items:e.orderBy(e.providersInternal,e.sortParams).slice(e.skip,e.skip+e.take),groups:e.providerTableGroups,"no-actionbar":!1,"search-placeholder":"Search Name, FRN","add-button-text":"Add New Provider",activeGroup:e.activeProviderTableGroup,"onUpdate:activeGroup":s[1]||(s[1]=t=>e.activeProviderTableGroup=t),sort:e.sortParams,handleSortChange:e.handleSort,handlePageChange:e.handlePage,pageable:!0,skip:e.skip,take:e.take,total:((C=e.providersInternal)==null?void 0:C.length)??0,onAdd:e.emitAddNewProvider,onFilter:e.emitFilterProvidersByProductsStatus,onSearch:e.emitSearchProvidersByNameFRN,onClear:e.emitClearFilters,onClose:e.emitCloseFilter,onAddExport:e.exportToPdf},{"cell-details.name":r(({item:{details:t}})=>[i(P,{rounded:"full",type:"text",text:(t==null?void 0:t.name)??""},{text:r(()=>[c(o(t==null?void 0:t.name),1)]),_:2},1032,["text"])]),"cell-details.fcaFirmRefNo":r(({item:{details:t}})=>[d("div",te,[t!=null&&t.fcaFirmRefNo?(n(),l("a",{key:0,href:e.helperService.generateFCASearchUrl((t==null?void 0:t.fcaFirmRefNo)??""),target:"_blank",rel:"noopener noreferrer"},o(t==null?void 0:t.fcaFirmRefNo),9,se)):q("",!0)])]),"cell-praAuthorised":r(({item:{details:t}})=>[t!=null&&t.praAuthorised?(n(),l("img",re)):(n(),l("img",ie))]),"cell-products":r(({item:{products:t}})=>[c(o("")),d("span",oe,o((t==null?void 0:t.length)>1?"+2":""),1)]),"cell-introducer":r(({item:{representative:t}})=>[i(P,{rounded:"full",type:"text",text:`${t==null?void 0:t.forename} ${t==null?void 0:t.surname}`},{text:r(()=>[c(o(t==null?void 0:t.forename)+" "+o(t==null?void 0:t.surname),1)]),_:2},1032,["text"])]),"cell-status":r(({item:{status:t}})=>[i(w,{themeColor:t==="Complete"?"success-tint":"warning-tint"},{default:r(()=>[c(o(t),1)]),_:2},1032,["themeColor"])]),"cell-tasks":r(({item:{tasks:t}})=>[d("div",ae,[d("span",ne,o((t==null?void 0:t.length)??0),1)])]),"cell-actions":r(({item:{id:t}})=>[d("div",le,[i(g,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"View",class:"ActionButton",onClick:h=>e.emitViewProviderFromList(t)},{default:r(()=>[i(v,{symbol:"eye",size:"20"})]),_:2},1032,["onClick"]),i(g,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"Edit",class:"ActionButton",onClick:h=>e.emitEditProviderList(t)},{default:r(()=>[i(v,{symbol:"edit-pen",size:"20"})]),_:2},1032,["onClick"])])]),"filter-fields":r(()=>[i(I,{name:"products",id:e.setUniqueIdentifier("-products"),label:"Products",class:"mb-3",dataItems:e.getProducts(),isRequired:!1,value:e.selectedProducts,onOnValueChange:s[0]||(s[0]=t=>{e.selectedProducts=t}),isValueReactive:!0,isDataLoadedCompletely:!e.isInitializing},null,8,["id","dataItems","value","isDataLoadedCompletely"]),i(y,{name:"status",id:e.setUniqueIdentifier("-status"),label:"Status",class:"mb-3",dataItems:e.getStatuses(),value:e.selectedStatus,isRequired:!1,onOnValueChange:e.handleStatusChange,isValueReactive:!0,isDataLoadedCompletely:!e.isInitializing},null,8,["id","dataItems","value","onOnValueChange","isDataLoadedCompletely"])]),card:r(()=>[d("div",de,[(n(!0),l(b,null,U(e.providers,(t,h)=>{var _,F;return n(),l("div",{class:"col-lg-4",key:t.id},[i(R,{cardId:t.id,cardName:(_=t.details)==null?void 0:_.name,cardfcaFirmRefNo:(F=t.details)==null?void 0:F.fcaFirmRefNo,cardTasks:t.tasks,cardIndex:h,onViewProvider:e.emitViewProviderFromCard,onEditProvider:e.emitProviderFromCard,onViewTasks:e.emitViewTasksFromCard},null,8,["cardId","cardName","cardfcaFirmRefNo","cardTasks","cardIndex","onViewProvider","onEditProvider","onViewTasks"])])}),128))])]),_:1},8,["columns","items","groups","activeGroup","sort","handleSortChange","handlePageChange","skip","take","total","onAdd","onFilter","onSearch","onClear","onClose","onAddExport"])],64)}const pe=z(ee,[["render",ue],["__scopeId","data-v-6faf6303"]]);export{pe as default};