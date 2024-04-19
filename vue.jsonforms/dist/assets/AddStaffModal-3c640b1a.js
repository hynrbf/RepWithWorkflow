import{_ as oe}from"./ModalComponent.vue_vue_type_script_setup_true_lang-fe9b98ed.js";import{j as ae,bo as ne,cF as le,cG as ie,cE as K,G as se,am as re,cH as de,D as me,I as pe,cI as ue,ay as ye,A as d,a3 as fe,cJ as ce,as as he,au as Re,av as ge,bd as Ce,_ as Ie,aA as b,o as h,q as B,w as l,n,m as v,P as p,y as A,x as R,aB as ve,r as D,a0 as Se,N as be,O as Ae,bh as De,aO as Te,Q as Fe,L as Le,b2 as _e,bp as we,cK as Me,cL as Ne,cM as Oe,aL as Ve,aM as Ee,aT as ke,aU as qe,a5 as $e,a6 as ze}from"./index-9d6d511f.js";import{_ as je}from"./OverlayLoader-e5a9021a.js";import{_ as H,R as Ue,D as Ke}from"./DropdownListItemModel-0538c8e6.js";const Be=ae({components:{KendoDropdownListWithTypeComponent:H,DynamicAvatar:ne,FcaRoleItemTemplate:le,FcaRoleHeaderTemplate:ie},props:{employee:{type:Object,default:new K},employeeValues:{type:Array,default:()=>[]},saving:{type:Boolean,default:!1},isEdit:{type:Boolean,default:!1}},setup(){const e=se(),t=re(),{changeLifeCycleName:a}=t,r=de(),{persistPrimaryRoles:m,persistOtherRoles:y,clearAllRoles:u,getPrimaryRoles:c,getOtherRoles:f}=r;return{changeLifeCycleName:a,modalElement:e,persistPrimaryRoles:m,persistOtherRoles:y,clearAllRoles:u,getPrimaryRoles:c,getOtherRoles:f}},data(){return{isInitializing:!0,additionalRole:"",primaryRolesFromStore:[],otherRolesObjFromStore:[],originalPrimaryRoles:[],helperService:me.resolve(pe.name),employeeInternal:this.employee}},computed:{Role(){return Ue},Employee(){return K},ProductType(){return ue},ContactNumber(){return ye},DropdownListItemModel(){return Ke},AppConstants(){return d},lineManagers(){return this.employeeValues.map(t=>{if(t.id===this.employee.id)return{label:"",value:""};const a=this.mapEmployeeToDropdownListItem(t);return a||{label:"",value:""}}).filter(t=>t.label)},isMinimumFieldsSupplied(){var e,t,a;return!((e=this.employeeInternal)!=null&&e.firstName)||!((t=this.employeeInternal)!=null&&t.lastName)||!((a=this.employeeInternal)!=null&&a.email)?!1:this.employeeInternal.firstName.length>0&&this.employeeInternal.lastName.length>0&&this.helperService.checkIfEmailFormatIsValid(this.employeeInternal.email)}},watch:{employeeInternal(e,t){return this.isEdit||this.clearAllRoles(),this.setupRoles(),e}},beforeMount(){this.changeLifeCycleName(d.pageLifeCycleNameCreated)},mounted(){this.clearAllRoles(),this.setupRoles(),this.isInitializing=!1,this.changeLifeCycleName(d.pageLifeCycleNameMounted)},unmounted(){this.clearAllRoles()},methods:{setupRoles(){const e=fe.getOrgStructureRoles();let t=[];e&&e.length&&(e.forEach(a=>{var y,u,c,f,C,I,g;let r;a===((u=(y=this.employeeInternal)==null?void 0:y.primaryRole)==null?void 0:u.name)?r={name:(c=this.employeeInternal.primaryRole)==null?void 0:c.name,state:(f=this.employeeInternal.primaryRole)==null?void 0:f.state,isModified:(C=this.employeeInternal.primaryRole)==null?void 0:C.isModified,isFcaAuthorised:(I=this.employeeInternal.primaryRole)==null?void 0:I.isFcaAuthorised,isPending:(g=this.employeeInternal.primaryRole)==null?void 0:g.isPending}:r={name:a,isFcaAuthorised:!1,state:d.removedState,isModified:!1,isPending:!1};const m=ce.cloneDeep(r);this.originalPrimaryRoles.push(m),t.push({label:a,value:a,raw:r})}),this.persistPrimaryRoles(t)),this.persistOtherRoles(t),this.primaryRolesFromStore=this.getPrimaryRoles(),this.otherRolesObjFromStore=this.getOtherRoles()},onLineManagerChange(e,t){t&&(e.lineManager=t.raw)},mapEmployeeToDropdownListItem(e){if(!e)return null;const t=`${e.firstName} ${e.lastName}`;return{label:t,value:t,raw:e}},mapRoleToDropdownListItem(e){return{label:e.name,value:e.name,raw:e}},mapRolesToDropdownListItems(e){return e!=null&&e.length?e.map(t=>({label:t.name,value:t.name,raw:t})):[]},onPrimaryRoleChange(e,t){t&&(e.primaryRole=t.raw)},onOtherRoleChange(e,t){t&&(e.otherRoles=t.map(a=>a.raw))},handleRoleAction(e,t,a,r,m){let y=document.querySelector(".k-animation-container.k-animation-container-relative.k-animation-container-shown");y&&(y.style.display="none",he({title:"Confirm",content:a,confirmButtonText:r,confirmButtonThemeColor:m,onConfirm:()=>{var u;Re({type:ge.SUCCESS,content:"Request Submitted.",interval:2e3}),t?this.employeeInternal.primaryRole=e:(u=this.employeeInternal.otherRoles)==null||u.push(e),this.updateRoleState(e,t)},onClose:()=>{}}))},addRoleToFca(e,t){if(e.state===d.addedState){this.updateRoleState(e,t);return}const a="This action requires a Variation of Permission Application to the FCA. Are you sure you wish to begin performing this activity?";this.handleRoleAction(e,t,a,"Confirm & Add")},removeRoleFromFca(e,t){if(e.state===d.removedState){this.updateRoleState(e,t);return}const a="This action requires a Variation of Permission Application to the FCA. Are you sure you no longer wish to perform this activity?";this.handleRoleAction(e,t,a,"Confirm & Remove","error")},toggleRoleIsPending(e,t){e.isPending=!e.isPending,e.isModified=this.checkIfModified(e)},checkIfModified(e){const t=this.originalPrimaryRoles.find(a=>a.name===e.name);return t!=null&&t.isModified?!0:e.isPending!==(t==null?void 0:t.isPending)||e.state!==t.state},updateRoleState(e,t){e.state=e.state===d.addedState?d.removedState:d.addedState,e.isModified=this.checkIfModified(e)},getRoleStatusStyle(e){var r;if(!e)return"";const t=(r=this.primaryRolesFromStore)==null?void 0:r.find(m=>(m==null?void 0:m.value)===e);if(!t)return"";const a=this.getRoleObj(t);return a?a!=null&&a.isModified?"modified-role":a!=null&&a.isFcaAuthorised?"authorised-role":"":""},getRoleObj(e){if(e&&e)return e.raw},addCustom(e,t){this.persistPrimaryRoles([]),this.primaryRolesFromStore.push({label:e,value:e,raw:{name:e,state:d.removedState,isFcaAuthorised:!1,isPending:!1,isModified:!1}}),this.otherRolesObjFromStore.push({label:e,value:e,raw:{name:e,state:d.removedState,isFcaAuthorised:!1,isPending:!1,isModified:!1}})},onProductTypesChange(e,t){t&&(e.productType=t)},handleRequestStaffToCompleteDetails(e){this.$emit("requestStaffToCompleteDetails",{...this.employeeInternal||{},...e})},handleSubmit(){var e,t,a,r;this.employeeInternal.id||(this.employeeInternal.id=Ce()),this.employeeInternal.tasks=[{name:"Implement compliance tools",description:"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",dueDate:this.helperService.dateToEpoch(new Date("01/31/2024"))}],this.$emit("submit",this.employeeInternal),(r=(a=(t=(e=this.$refs)==null?void 0:e.formElement)==null?void 0:t.$el)==null?void 0:a.requestSubmit)==null||r.call(a)},setUniqueIdentifier(e){return`${d.organizationalStructureRoute}${e}`.replace(/\s+/g,"").replace("/","")}}});const He=e=>($e("data-v-f1e159d1"),e=e(),ze(),e),Ge={key:0,class:"d-flex flex-column"},We={class:"d-flex align-self-center",style:{"margin-left":"14px"}},Je=He(()=>p("text",{class:"edit-text"}," Edit",-1)),Pe={class:"d-flex flex-column",style:{gap:"20px"}},Qe={class:"d-flex",style:{gap:"15px"}},Xe={key:0,class:"hstack gap-2"},Ye={key:1,class:"hstack gap-2"},Ze={class:"col-6"},xe=["title"],et={key:0,href:"#"},tt={class:"col-6"},ot={class:"d-flex",style:{gap:"15px"}},at={class:"text-right",style:{"margin-top":"20px"}};function nt(e,t,a,r,m,y){const u=b("DynamicAvatar"),c=be,f=Ae,C=De,I=Te,g=Fe,T=H,F=b("FcaRoleHeaderTemplate"),L=b("FcaRoleItemTemplate"),G=Le,_=_e,W=we,J=Me,P=Ne,Q=Oe,X=Ve,Y=Ee,Z=je,x=ke,ee=qe,te=oe;return h(),B(te,{ref:"modalElement",title:`${e.isEdit?"":"Add New Staff"}`,width:"750"},{default:l(()=>[n(ee,null,{default:l(()=>[n(x,{ref:"formElement"},{default:l(()=>[n(Z,{loading:e.saving},{default:l(()=>{var w,M,N,O,V,E,k,q,$,z,j,U;return[e.isEdit?(h(),v("div",Ge,[p("div",We,[n(u,{size:"large",type:"image",rounded:"full",avatarCustomStyle:{flexBasis:"60px",height:"60px"},imageSrc:(w=e.employee)==null?void 0:w.img_id,imageAlt:(M=e.employee)==null?void 0:M.img_id},null,8,["imageSrc","imageAlt"]),n(f,{class:"edit-icon-button",type:"button",size:"small",shape:"square","theme-color":"light",title:"Edit"},{default:l(()=>[n(c,{symbol:"edit-pen",size:"20",style:{color:"var(--brand-color-brand-primary)"}})]),_:1})]),Je,n(C,{height:1,width:690,spacing:0,style:{"margin-top":"20px","margin-bottom":"20px"}})])):A("",!0),p("div",Pe,[p("div",Qe,[n(I,{class:"col",name:e.setUniqueIdentifier("-title"),id:"title",isRequired:!1,label:"Title",value:(N=e.employeeInternal)==null?void 0:N.title,onOnValueChange:t[0]||(t[0]=o=>e.employeeInternal.title=o),isDataLoadedCompletely:!e.isInitializing,isValueReactive:!0},null,8,["name","value","isDataLoadedCompletely"]),n(g,{id:e.setUniqueIdentifier("-foreName"),class:"col",name:"foreName",label:"Forename(s)",placeholder:"John",value:(O=e.employeeInternal)==null?void 0:O.firstName,onOnValueChange:t[1]||(t[1]=o=>e.employeeInternal.firstName=o),isDataLoadedCompletely:!e.isInitializing,isValueReactive:!0},null,8,["id","value","isDataLoadedCompletely"]),n(g,{id:e.setUniqueIdentifier("-surName"),class:"col",name:"surName",label:"Surname",placeholder:"Doe",value:(V=e.employeeInternal)==null?void 0:V.lastName,onOnValueChange:t[2]||(t[2]=o=>e.employeeInternal.lastName=o),isDataLoadedCompletely:!e.isInitializing,isValueReactive:!0},null,8,["id","value","isDataLoadedCompletely"])]),n(T,{id:e.setUniqueIdentifier("-lineManager"),name:"lineManager",label:"Line Manager",disabled:(E=e.employeeInternal)==null?void 0:E.isRoot,dataItems:e.lineManagers,value:(k=e.employeeInternal)!=null&&k.lineManager?e.mapEmployeeToDropdownListItem(e.employeeInternal.lineManager):null,onOnValueChange:t[3]||(t[3]=o=>e.onLineManagerChange(e.employee,o)),isDataLoadedCompletely:!e.isInitializing,isValueReactive:!0},{display:l(({value:o})=>{var i;return[(i=e.employeeInternal)!=null&&i.isRoot?(h(),v("div",Xe,R(e.AppConstants.root),1)):(h(),v("div",Ye,R(o==null?void 0:o.label),1))]}),_:1},8,["id","disabled","dataItems","value","isDataLoadedCompletely"]),n(T,{id:e.setUniqueIdentifier("-primaryRole"),name:"primaryRole",label:"Primary Role",value:(q=e.employeeInternal)!=null&&q.primaryRole?e.mapRoleToDropdownListItem(e.employeeInternal.primaryRole):null,placeholder:"Select Primary Role",dataItems:e.primaryRolesFromStore,addable:!0,bypassDefaultOnSelectEvent:"",onAddCustom:e.addCustom,onOnValueChange:t[8]||(t[8]=o=>e.onPrimaryRoleChange(e.employee,o)),isDataLoadedCompletely:!e.isInitializing,isValueReactive:!0},{header:l(()=>[n(F)]),display:l(({value:o,clickEvent:i})=>[n(L,{role:e.getRoleObj(o),clickEvent:i,onAddRoleToFca:t[4]||(t[4]=s=>e.addRoleToFca(s,!0)),onRemoveRoleFromFca:t[5]||(t[5]=s=>e.removeRoleFromFca(s,!0)),onToggleRoleIsPending:t[6]||(t[6]=s=>e.toggleRoleIsPending(s,!0))},null,8,["role","clickEvent"])]),valueDisplayTemplate:l(({value:o})=>{var i,s;return[n(G,{style:ve((i=e.getRoleObj(o))!=null&&i.isModified?"color: var(--color-warning-700)":(s=e.getRoleObj(o))!=null&&s.isFcaAuthorised?"color: var(--color-success-600)":"")},{default:l(()=>{var S;return[D(R((S=e.getRoleObj(o))==null?void 0:S.name),1)]}),_:2},1032,["style"])]}),footerTemplate:l(({onAdd:o})=>[p("div",Ze,[n(_,{modelValue:e.additionalRole,"onUpdate:modelValue":t[7]||(t[7]=i=>e.additionalRole=i),placeholder:"Others",onAdd:i=>o(e.additionalRole)},null,8,["modelValue","onAdd"])])]),_:1},8,["id","value","dataItems","onAddCustom","isDataLoadedCompletely"]),n(P,{id:e.setUniqueIdentifier("-otherRole"),name:"otherRole",label:"Other Roles",value:($=e.employee)!=null&&$.otherRoles?e.mapRolesToDropdownListItems(e.employee.otherRoles):[],onOnValueChange:t[13]||(t[13]=o=>e.onOtherRoleChange(e.employee,o)),placeholder:"Select Other Applicable Roles",dataItems:e.otherRolesObjFromStore,isRequired:!1,onAddCustom:e.addCustom,isDataLoadedCompletely:!e.isInitializing,isValueReactive:!0,bypassDefaultOnSelectEvent:"",addable:!0},{header:l(()=>[n(F,{"is-multi-select":""})]),display:l(({item:o,clickEvent:i})=>[n(L,{role:e.getRoleObj(o),clickEvent:i,onAddRoleToFca:t[9]||(t[9]=s=>e.addRoleToFca(s,!1)),onRemoveRoleFromFca:t[10]||(t[10]=s=>e.removeRoleFromFca(s,!1)),onToggleRoleIsPending:t[11]||(t[11]=s=>e.toggleRoleIsPending(s,!1)),isMultiSelect:""},null,8,["role","clickEvent"])]),tagTemplate:l(({tagProps:o,item:i,onClose:s})=>[n(J,{"anchor-element":"target",position:"top"},{default:l(()=>[n(W,{themeColor:"white",size:"lg",class:Se(["font-weight-semi-bold",e.getRoleStatusStyle(o==null?void 0:o.tagData.text)]),textClass:"is-truncated",closeable:"",onClose:S=>s(i)},{default:l(()=>[p("span",{class:"is-truncated",title:o==null?void 0:o.tagData.text},R(o==null?void 0:o.tagData.text),9,xe)]),_:2},1032,["class","onClose"])]),_:2},1024),(o==null?void 0:o.dataItems.length)>4&&(o==null?void 0:o.dataItems.length)-1===(o==null?void 0:o.index)?(h(),v("a",et,[p("b",null,"+"+R((o==null?void 0:o.dataItems.length)-4),1)])):A("",!0)]),footerTemplate:l(({onAdd:o})=>[p("div",tt,[n(_,{modelValue:e.additionalRole,"onUpdate:modelValue":t[12]||(t[12]=i=>e.additionalRole=i),placeholder:"Others",onAdd:i=>o(e.additionalRole)},null,8,["modelValue","onAdd"])])]),_:1},8,["id","value","dataItems","onAddCustom","isDataLoadedCompletely"]),n(Q,{id:e.setUniqueIdentifier("-productType"),name:"productType",label:"Product Type",pageName:"Organisational Structure Chart",value:(z=e.employee)==null?void 0:z.productType,onOnValueChange:t[14]||(t[14]=o=>e.onProductTypesChange(e.employee,o)),isDataLoadedCompletely:!e.isInitializing,isValueReactive:!0},null,8,["id","value","isDataLoadedCompletely"]),p("div",ot,[n(X,{id:e.setUniqueIdentifier("-emailAddress"),class:"col",name:"emailAddress",label:"Email Address",placeholder:"name@domain.com",value:(j=e.employeeInternal)==null?void 0:j.email,onOnValueChange:t[15]||(t[15]=o=>e.employeeInternal.email=o),isDataLoadedCompletely:!e.isInitializing,isValueReactive:!0},null,8,["id","value","isDataLoadedCompletely"]),n(Y,{id:e.setUniqueIdentifier("-contactNo"),class:"col",name:"contactNo",label:"Contact Number",value:(U=e.employeeInternal)==null?void 0:U.contactNumber,onOnValueChange:t[16]||(t[16]=o=>e.employeeInternal.contactNumber=o),isValueReactive:!0,isDataLoadedCompletely:!e.isInitializing},null,8,["id","value","isDataLoadedCompletely"])])]),p("div",at,[e.isEdit?A("",!0):(h(),B(f,{key:0,style:{"margin-right":"10px","font-weight":"600"},type:"button","fill-mode":"outline","theme-color":"primary",disabled:!e.isMinimumFieldsSupplied,onClick:e.handleRequestStaffToCompleteDetails},{default:l(()=>[D(" Request Staff to Complete Details ")]),_:1},8,["disabled","onClick"])),n(f,{type:"button","theme-color":"primary",disabled:e.saving||!e.isMinimumFieldsSupplied,onClick:e.handleSubmit},{default:l(()=>[D(R(e.isEdit?"Save Changes":"Save & Add New Staff"),1)]),_:1},8,["disabled","onClick"])])]}),_:1},8,["loading"])]),_:1},512)]),_:1})]),_:1},8,["title"])}const dt=Ie(Be,[["render",nt],["__scopeId","data-v-f1e159d1"]]);export{dt as default};