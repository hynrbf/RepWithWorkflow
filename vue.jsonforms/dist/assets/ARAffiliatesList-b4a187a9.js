import{j as V,D as I,I as $,d6 as T,bj as G,bl as P,a3 as S,A as B,_ as D,o as r,m as n,n as a,w as s,bg as L,bm as E,r as m,x as l,P as c,y as q,J as _,K as z,bn as O,bi as U,bo as K,bp as j,N as H,O as J,bq as M,a2 as W,bs as Q,br as X}from"./index-9d6d511f.js";import{_ as Y,a as Z}from"./rejected-grey-icon-2a930ba1.js";const x=V({name:"ARAffiliatesList",props:{affiliatesValues:{type:Array,default:()=>[]}},data(){return{affiliatesInternal:[],selectedProducts:[],selectedStatus:{label:"",value:""},activeAffiliateTableGroup:"current",affiliateTableGroups:[{title:"Current",value:"current"},{title:"All",value:"all"}],helperService:I.resolve($.name),sortParams:[],skip:0,take:7,itemsToPDF:[],isGridVisible:!1}},mounted(){this.affiliatesInternal=this.affiliatesValues},computed:{AffiliateDetails(){return T},ProviderRepresentative(){return G},affiliateColumns(){return[{field:"details.name",title:"Affiliate Name",width:200},{field:"details.firmReferenceNumber",title:"FRN",width:200},{field:"details.isPRAAuthorised",title:"PRA Authorised",width:200},{field:"products",title:"Products",width:160},{field:"affiliate",title:"Affiliate Representative",width:200},{field:"status",title:"Status",width:140},{field:"tasks",title:"Task(s)",width:160,sortable:!1},{field:"actions",title:" ",width:160,sortable:!1}]}},methods:{orderBy:P,emitAddNewAffiliate(){this.$emit("add-new-affiliate")},emitViewAffiliateFormList(e){this.$emit("view-affiliate-list",e)},emitEditAffiliateList(e){this.$emit("edit-affiliate-list",e)},emitViewAffiliateFromCard(e){this.$emit("view-affiliate-card",e)},emitAffiliateFromCard(e){this.$emit("edit-affiliate-card",e)},emitFilterAffiliatesByProductsStatus(){this.affiliatesInternal=this.affiliatesValues.filter(e=>{const i=e.products.map(u=>u.name),o=this.selectedProducts.every(u=>i.includes(u)),d=e.status===this.selectedStatus.value;return o&&d})},emitSearchAffiliatesByNameFRN(e){if(!e){this.affiliatesInternal=this.affiliatesValues;return}this.affiliatesInternal=this.affiliatesValues.filter(i=>{if(!i.details.name||!i.details.firmReferenceNumber)return!1;const o=i.details.name.toLowerCase().includes(e.toLowerCase()),d=i.details.firmReferenceNumber.toLowerCase().includes(e.toLowerCase());return o||d})},emitClearFilters(){this.selectedProducts=[],this.selectedStatus={label:"",value:""}},emitCloseFilter(){this.affiliatesInternal=this.affiliatesValues,this.emitClearFilters()},exportToPdf(e){const i=this.formatAffiliatesPdfData(e);this.itemsToPDF=P(i,this.sortParams),this.$refs.gridPdfExport.save(this.itemsToPDF)},formatAffiliatesPdfData(e){return e.map(i=>{var o;return{...i,"details.name":`${(o=i.details)==null?void 0:o.name} `,"details.firmReferenceNumber":i.details.firmReferenceNumber,"details.isPRAAuthorised":i.details.isPRAAuthorised,products:i.products,affiliate:`${i.representative.forename} ${i.representative.surname}`,status:i.status,tasks:i.tasks}})},getProducts(){return S.getProducts()},getStatuses(){return S.getStatuses().map(e=>({label:e,value:e}))},handleStatusChange(e){this.selectedStatus=e},handleSort(e){this.sortParams=e.sort},handlePage(e){this.skip=e.page.skip,this.take=e.page.take},setUniqueIdentifier(e){const i=`${B.arAffiliatesRoute}-list${e}`;return this.helperService.removeStringSpacesThenSlash(i)}}}),ee={style:{color:"#309161","font-size":"14px","font-weight":"400","text-decoration":"underline"}},te=["href"],ie={key:0,src:Y,alt:"Authorised",title:"Authorised",width:"16",height:"16"},se={key:1,src:Z,alt:"Non-Authorised",title:"Non-Authorised",width:"16",height:"16"},ae={class:"d-inline-flex gap-2"},oe={class:"row mt-2"};function re(e,i,o,d,u,ne){var g;const w=O,N=U,h=K,k=j,p=H,A=J,R=M,F=W,v=Q,y=X;return r(),n(_,null,[a(N,{ref:"gridPdfExport"},{default:s(()=>[L(a(w,{"data-items":e.itemsToPDF,columns:e.affiliateColumns},null,8,["data-items","columns"]),[[E,e.isGridVisible]])]),_:1},512),a(y,{isSortable:"",columns:e.affiliateColumns,items:e.orderBy(e.affiliatesInternal,e.sortParams).slice(e.skip,e.skip+e.take),groups:e.affiliateTableGroups,"no-actionbar":!1,"search-placeholder":"Search Name, FRN","add-button-text":"Add New Affiliate",activeGroup:e.activeAffiliateTableGroup,"onUpdate:activeGroup":i[1]||(i[1]=t=>e.activeAffiliateTableGroup=t),sort:e.sortParams,handleSortChange:e.handleSort,handlePageChange:e.handlePage,pageable:!0,skip:e.skip,take:e.take,total:((g=e.affiliatesInternal)==null?void 0:g.length)??0,onAdd:e.emitAddNewAffiliate,onFilter:e.emitFilterAffiliatesByProductsStatus,onSearch:e.emitSearchAffiliatesByNameFRN,onClear:e.emitClearFilters,onClose:e.emitCloseFilter,onAddExport:e.exportToPdf},{"cell-details.name":s(({item:{details:t}})=>[a(h,{rounded:"full",type:"text",text:(t==null?void 0:t.name)??""},{text:s(()=>[m(l(t==null?void 0:t.name),1)]),_:2},1032,["text"])]),"cell-details.firmReferenceNumber":s(({item:{details:t}})=>[c("div",ee,[t!=null&&t.firmReferenceNumber?(r(),n("a",{key:0,href:e.helperService.generateFCASearchUrl((t==null?void 0:t.firmReferenceNumber)??""),target:"_blank",rel:"noopener noreferrer",style:{color:"#309161","font-size":"14px","font-weight":"400","text-decoration":"underline"}},l(t==null?void 0:t.firmReferenceNumber),9,te)):q("",!0)])]),"cell-details.isPRAAuthorised":s(({item:{details:t}})=>[t!=null&&t.isPRAAuthorised?(r(),n("img",ie)):(r(),n("img",se))]),"cell-affiliate":s(({item:{representative:t}})=>[a(h,{rounded:"full",type:"text",text:`${t==null?void 0:t.forename} ${t==null?void 0:t.surname}`},{text:s(()=>[m(l(t==null?void 0:t.forename)+" "+l(t==null?void 0:t.surname),1)]),_:2},1032,["text"])]),"cell-status":s(({item:{status:t}})=>[a(k,{"theme-color":t==="Complete"?"success-tint":"warning-tint"},{default:s(()=>[m(l("Onboarding"))]),_:2},1032,["theme-color"])]),"cell-actions":s(({item:{id:t}})=>[c("div",ae,[a(A,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"View",class:"ActionButton",onClick:f=>e.emitViewAffiliateFormList(t)},{default:s(()=>[a(p,{symbol:"eye",size:"20"})]),_:2},1032,["onClick"]),a(A,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"Edit",class:"ActionButton",onClick:f=>e.emitEditAffiliateList(t)},{default:s(()=>[a(p,{symbol:"edit-pen",size:"20"})]),_:2},1032,["onClick"])])]),"filter-fields":s(({})=>[a(R,{name:"products",id:e.setUniqueIdentifier("-products"),label:"Products","data-items":e.getProducts(),class:"mb-3",value:e.selectedProducts,onOnValueChange:i[0]||(i[0]=t=>{t.length>0&&(e.selectedProducts=t)})},null,8,["id","data-items","value"]),a(F,{name:"status",id:e.setUniqueIdentifier("-status"),label:"Status","data-items":e.getStatuses(),class:"mb-3",value:e.selectedStatus,onOnValueChange:e.handleStatusChange},null,8,["id","data-items","value","onOnValueChange"])]),card:s(()=>[c("div",oe,[(r(!0),n(_,null,z(e.affiliatesValues,(t,f)=>{var b,C;return r(),n("div",{class:"col-lg-4",key:t.id},[a(v,{cardId:t.id,cardName:(b=t.details)==null?void 0:b.name,cardfcaFirmRefNo:(C=t.details)==null?void 0:C.firmReferenceNumber,cardTasks:t.tasks,cardIndex:f,onViewProvider:e.emitViewAffiliateFromCard,onEditProvider:e.emitAffiliateFromCard},null,8,["cardId","cardName","cardfcaFirmRefNo","cardTasks","cardIndex","onViewProvider","onEditProvider"])])}),128))])]),_:1},8,["columns","items","groups","activeGroup","sort","handleSortChange","handlePageChange","skip","take","total","onAdd","onFilter","onSearch","onClear","onClose","onAddExport"])],64)}const ue=D(x,[["render",re]]);export{ue as default};
