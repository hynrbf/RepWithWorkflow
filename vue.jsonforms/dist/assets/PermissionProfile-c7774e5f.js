import{j as U,C as Q,Z as L,G as X,bF as j,o as F,q as ee,w as C,P as h,x as f,a0 as se,m as x,n as d,H as _,A as n,bG as v,a_ as q,_ as z,D as T,bz as ie,ac as te,I as oe,bH as M,af as ne,l as D,ag as re,b3 as ae,bd as O,am as me,u as ce,ao as ue,au as N,av as $,as as E,at as V,bI as de,aA as le,J as he,aC as fe,aH as pe,b9 as ge,r as w,a9 as ve,aG as be}from"./index-9d6d511f.js";import{_ as Pe}from"./InfoTableComponent.vue_vue_type_script_setup_true_lang-a9fe18e7.js";import{_ as Ce,a as Se}from"./rejected-grey-icon-2a930ba1.js";const Ae={key:0,src:Ce,alt:"Authorised",title:"Authorised",width:"16",height:"16"},ye={key:1,src:Se,alt:"Non-Authorised",title:"Non-Authorised",width:"16",height:"16"},_e=U({__name:"PermissionsTable",props:{permissionGroups:{default:()=>[]},permissionsFromFca:{},customerPermissions:{default:()=>[]},authorised:{type:Boolean}},emits:["change"],setup(e,{emit:i}){const a=e,{permissionGroups:c,permissionsFromFca:t,customerPermissions:l,authorised:S}=Q(a),y=L(()=>{var s;return((s=t.value)==null?void 0:s.permissionNames)||[]}),p=L(()=>{try{return JSON.parse(`${t.value.raw}`)}catch{return null}}),b=X([{field:"name",title:" "},{field:"fcaAuthorised",title:"FCA Authorised"},{field:"add",title:"Add"},{field:"pending",title:"Pending"}]);j(()=>{S.value?b.value.push({field:"remove",title:"Remove"}):b.value=b.value.filter(({field:s})=>s!=="remove")});const B=L(()=>c.value.map(s=>({title:s.permissionGroupName,items:s.subPermissions.map(({name:r,displayText:o,categoryName:u})=>({name:o||r||"",permissionName:r,categoryName:u}))}))),A=(s,r)=>{if(!y.value.length||!s||!r)return!1;if(r===n.PermissionCategoryInsuranceBroker){const u=/\[([^\]]+)\]$/.exec(s);if(u&&u.length>0){if(!p.value)return!1;let m=s.replace(u[0],"").trim(),g=u[1],P=!1;return Object.keys(p.value).forEach(R=>{var G;if(R.toLowerCase()===(m==null?void 0:m.toLowerCase())){const Z=(G=p==null?void 0:p.value)==null?void 0:G[R];P=k(Z,g)}}),P}}return K(s)},k=(s,r)=>{if(!s)return!1;if(typeof s!="object")return s===r;if(Array.isArray(s)){for(const o of s)if(k(o,r))return!0}if(typeof s=="object"){for(const o in s)if(k(s[o],r))return!0}return!1},K=s=>{let r=y.value.find(o=>o.toLowerCase()===s.toLowerCase());return!!(r&&r.trim().length>0)},H=(s,r)=>{if(A(s,r))return!0;const o=l.value.find(({subPermissionName:u})=>u===s);return o?o.hasPendingApplication:!1},J=(s,r)=>!A(s,r),Y=(s,r)=>{if(A(s,r))return!0;const o=l.value.find(({subPermissionName:u})=>u===s);return o?o.isModified&&o.state===v.Added:!1},W=(s,r)=>{const o=l.value.find(({subPermissionName:u})=>u===s);return o?o.isModified:!1},I=(s,r)=>{const o=l.value.find(({subPermissionName:u})=>u===r);return o?!!(o.state===s&&o.isModified):!1};return(s,r)=>{const o=q,u=Pe;return F(),ee(u,{class:"PermissionsTable",columns:b.value,data:B.value},{"cell-name":C(({item:{name:m,permissionName:g,categoryName:P}})=>[h("span",{class:se([W(g,P)&&"is-edited"])},f(m),3)]),"cell-fcaAuthorised":C(({item:{permissionName:m,categoryName:g}})=>[A(m,g)?(F(),x("img",Ae)):(F(),x("img",ye))]),"cell-add":C(({item:{permissionName:m,categoryName:g}})=>[d(o,{class:"Checkbox Checkbox--plus",checked:I(_(v).Added,m),disabled:H(m,g),onChange:P=>i("change",_(n).seekAuthAdd,P.value,m)},null,8,["checked","disabled","onChange"])]),"cell-pending":C(({item:{permissionName:m,categoryName:g}})=>[d(o,{class:"Checkbox",checked:I(_(v).Pending,m),disabled:Y(m,g),onChange:P=>i("change",_(n).seekAuthPending,P.value,m)},null,8,["checked","disabled","onChange"])]),"cell-remove":C(({item:{permissionName:m,categoryName:g}})=>[d(o,{class:"Checkbox Checkbox--minus",checked:I(_(v).Removed,m),disabled:J(m,g),onChange:P=>i("change",_(n).seekAuthRemove,P.value,m)},null,8,["checked","disabled","onChange"])]),_:1},8,["columns","data"])}}});const Fe=z(_e,[["__scopeId","data-v-f8d423b1"]]),ke=U({name:"PermissionProfile",components:{PermissionsTable:Fe},data(){return{AppConstants:n,currentTab:0,tabs:[{title:"Mortgage Broker Authorisation Status",content:"tab-1"},{title:"Insurance and Protection Broker Authorisation Status",content:"tab-2"},{title:"Other Permissions",content:"tab-3"}],customerService:T.resolve(ie.name),appService:T.resolve(te.name),helperService:T.resolve(oe.name),selectedFcaFirm:null,customer:new M,fcaService:T.resolve(ne.name),definedGroupedPermissions:[],currentFcaSubPermissions:[],customerSubPermissions:[],subPermissionsFromFca:null,isLoading:!1,complianceFirmName:"",isShowSavingText:!1,eventBus:D("$eventBusService"),eventBusFormSaved:D("$eventBusService"),userSubmittedChangesService:T.resolve(re.name),isSavingAlertOpened:!1,debouncedAutoSaveFunction:()=>{}}},computed:{...ae(de,["currentFirmName"]),parsedPermissions(){const e=[];for(const i of this.customerSubPermissions){const{subPermissionName:a="",subPermissionDisplayText:c="",categoryName:t="",permissionGroupName:l=""}=i;if(!a.trim()){e.push({id:O(),permissionGroupName:i.permissionGroupName,categoryName:i.categoryName,subPermissions:[]});continue}const S=e.find(p=>p.permissionGroupName===l),y={name:a,displayText:c,categoryName:t};if(S)S.subPermissions.push(y);else{const p={id:O(),permissionGroupName:l,categoryName:t,subPermissions:[y]};e.push(p)}}return e},mortgageBrokerPermissions(){return this.getGroupedPermissions(n.PermissionCategoryMortgateBroker)},insuranceBrokerPermissions(){return this.getGroupedPermissions(n.PermissionCategoryInsuranceBroker)},otherPermissions(){return this.getGroupedPermissions(n.PermissionCategoryAdditional)},isFirmAuthorised(){var e;return(e=this.selectedFcaFirm)==null?void 0:e.isAuthorized}},setup(){const e=me(),{changeLifeCycleName:i}=e,{addComponentValidationValue:a,clearValidationValuesByPrefix:c}=ce(),{debounceFunction:t,setAutoSaveFunctionNotCompletedYet:l}=ue();return{changeLifeCycleName:i,addComponentValidationValue:a,clearValidationValuesByPrefix:c,debounceFunction:t,setAutoSaveFunctionNotCompletedYet:l}},created(){this.changeLifeCycleName(n.pageLifeCycleNameCreated),this.debouncedAutoSaveFunction=this.debounceFunction(this.saveInfoAsync)},async mounted(){this.complianceFirmName=await this.appService.getComplianceFirmNameAsync(),await this.setupData(),this.eventBus.emit(n.bottomBarEnableEvent,!0),this.eventBusFormSaved.on(n.formSavedEvent,e=>{this.handleSave(e)}),this.eventBus.on(n.formFieldPageLevelChangedEvent,()=>{this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi=!1,this.setAutoSaveFunctionNotCompletedYet(),this.debouncedAutoSaveFunction()}),this.changeLifeCycleName(n.pageLifeCycleNameMounted)},unmounted(){this.eventBus.emit(n.bottomBarEnableEvent,!1),this.eventBusFormSaved.off(n.formSavedEvent),this.eventBus.off(n.formFieldPageLevelChangedEvent),this.isLoading=!1},methods:{selectTab(e){this.currentTab=e.selected},async setupData(){var i;this.isLoading=!0,this.definedGroupedPermissions=await this.fcaService.getDefinedPermissionsAsync(),this.customer=this.appService.getCachedCustomer()??new M,this.selectedFcaFirm=this.customer.selectedCompany,this.currentFcaSubPermissions=this.customer.currentFcaPermissions,this.customerSubPermissions=this.customer.customerPermissions;const e=(i=this.selectedFcaFirm)==null?void 0:i.firmReferenceNo;if(!e){this.isLoading=!1;return}this.subPermissionsFromFca=await this.fcaService.getFirmPermissionsAsync(e).catch(()=>(N({type:$.ERROR,content:"Something went wrong.",interval:5e3}),{permissionNames:[],raw:""})).finally(()=>{this.isLoading=!1}),(!this.currentFcaSubPermissions.length||!this.customerSubPermissions.length)&&(this.setCurrentFcaPermissions(),this.setCustomerSubPermissions()),this.isLoading=!1},getGroupedPermissions(e){return this.parsedPermissions.filter(({categoryName:i})=>i===e)},handleSave(e){this.isSavingAlertOpened||(E({title:this.$t("common-alert-title"),content:this.$t("common-alert-content"),confirmButtonText:this.$t("common-alert-buttonText"),onConfirm:async()=>{await this.saveInfoAsync(e,!0)},onCancel:()=>{this.isSavingAlertOpened=!1},onClose:()=>{this.isSavingAlertOpened=!1}}),this.isSavingAlertOpened=!0)},async saveInfoAsync(e=!1,i=!1){var c;this.isLoading=i,this.isShowSavingText=i,this.userSubmittedChangesService.hasUserSubmittedChangesToRemoteApi=!0,this.isSavingAlertOpened=!1;const a=await this.customerService.getCustomerByEmailAsync(((c=this.customer)==null?void 0:c.email)??"");a.currentFcaPermissions=this.currentFcaSubPermissions,a.customerPermissions=this.customerSubPermissions,await this.customerService.saveCustomerAsync(JSON.stringify(a)).then(()=>{i&&N({type:$.SUCCESS,content:this.$t("common-notification-saved"),interval:n.notificationPopupTimeOut}),this.isLoading=!1,this.isShowSavingText=!1,e&&setTimeout(()=>this.eventBus.emit(n.autoNextEvent),n.notificationPopupTimeOut)}).catch(()=>{N({type:$.ERROR,content:"Something went wrong.",interval:5e3})}).finally(()=>{this.isLoading=!1,this.isShowSavingText=!1})},updatePermission(e,i,a){this.eventBus.emit(n.formFieldChangedEvent),this.eventBus.emit(n.formFieldPageLevelChangedEvent);const c=this.customerSubPermissions.findIndex(({subPermissionName:l="",permissionGroupName:S=""})=>a.toLowerCase()===l.toLowerCase()||a.toLowerCase()===S.toLowerCase());if(c<0)throw new Error(`${a} should not be null in customerSubPermissions`);const t=this.customerSubPermissions[c];switch(e){case n.seekAuthAdd:E({type:V.ALERT,title:"Confirm",content:t.isModified?this.$t("common-alert-permissionConfirmCancelAddText"):this.$t(this.isFirmAuthorised?"common-alert-permissionConfirmAddText":"common-alert-permissionConfirmAddTextUnAuthorised"),confirmButtonText:t.isModified?this.$t("common-alert-confirmAndCancelRequest"):this.$t("common-alert-confirmAndAdd"),confirmButtonThemeColor:t.isModified?"error":"primary",onConfirm:()=>{N({type:$.SUCCESS,content:t.isModified?this.$t("common-notification-CancelrequestSubmitted"):this.$t("common-notification-requestSubmitted"),interval:2e3}),t.state=v.Added,t.isModified=!t.isModified}});break;case n.seekAuthRemove:E({type:V.ALERT,title:"Confirm",content:t.isModified?this.$t("common-alert-permissionConfirmCancelRemoveText"):this.$t("common-alert-permissionConfirmRemoveText"),confirmButtonText:t.isModified?this.$t("common-alert-confirmAndCancelRequest"):this.$t("common-alert-confirmAndRemove"),confirmButtonThemeColor:"error",onConfirm:()=>{N({type:$.SUCCESS,content:t.isModified?this.$t("common-notification-CancelrequestSubmitted"):this.$t("common-notification-requestSubmitted"),interval:2e3}),t.state=v.Removed,t.isModified=!t.isModified}});break;case n.seekAuthPending:t.state=v.Pending,t.hasPendingApplication=!t.hasPendingApplication,t.isModified=!t.isModified;break;default:throw new Error("We don't support this action in updatePermissions")}},setCurrentFcaPermissions(){this.currentFcaSubPermissions=this.definedGroupedPermissions.map(e=>{const i=this.getPermissionState(e);return{id:e.id,permissionGroupName:e.permissionGroupName,categoryName:e.categoryName,subPermissionName:e.subPermissionName,subPermissionDisplayText:e.subPermissionDisplayText,state:i,hasPendingApplication:i===v.Added,isModified:!1}})},setCustomerSubPermissions(){this.customerSubPermissions=this.definedGroupedPermissions.map(e=>{const i=this.getPermissionState(e);return{id:e.id,permissionGroupName:e.permissionGroupName,categoryName:e.categoryName,subPermissionName:e.subPermissionName,subPermissionDisplayText:e.subPermissionDisplayText,state:i,hasPendingApplication:i===v.Added,isModified:!1}})},getPermissionState(e){const{subPermissionName:i="",permissionGroupName:a=""}=e,{permissionNames:c=[]}=this.subPermissionsFromFca;return c.find(l=>l.toLowerCase()===(i||a).toLowerCase())?v.Added:v.Removed},updateProgress(e){const i=this.helperService.removeStringSpacesThenSlash(n.permissionProfileRoute),a=`${i}-placeholder1`,c=`${i}-placeholder2`;this.clearValidationValuesByPrefix(i),this.addComponentValidationValue(a,{[a]:""}),e||this.addComponentValidationValue(c,{[c]:"error"})}},watch:{customerSubPermissions:{handler(e){e.some(i=>i.isModified)?this.updateProgress(!1):this.updateProgress(!0)},deep:!0,immediate:!0}}}),Te={key:0,class:"col-12 mb-3"},Ne={class:"InfoCheck"},$e={class:"InfoCheck"},we={class:"InfoCheck"},xe={key:1,class:"col-12 mb-3"},Be={class:"InfoCheck"},Ie={class:"InfoCheck"},Le={class:"InfoCheck"},Ee={class:"font-size-xl text-primary mb-4"},Re={class:"font-size-xl text-primary mb-4"},Ge={class:"font-size-xl text-primary mb-4"};function Me(e,i,a,c,t,l){const S=ve,y=fe,p=be,b=q,B=pe,A=le("PermissionsTable"),k=ge;return F(),x(he,null,[d(S,{ref:"kendoDialog"},null,512),d(y,{isLoading:e.isLoading,isShowSavingText:e.isShowSavingText},null,8,["isLoading","isShowSavingText"]),d(p),d(B,{title:"Permissions",routeName:e.helperService.removeStringSpacesThenSlash(e.AppConstants.permissionProfileRoute),"progress-bar":""},{description:C(()=>[e.isFirmAuthorised?(F(),x("div",Te,[h("p",null,f(e.$t("permissionPage-description",{currentFirmName:e.currentFirmName})),1),h("div",Ne,[d(b,{value:!1,class:"Checkbox Checkbox--plus is-unclickable"}),w(" "+f(e.$t("permissionPage-checkboxInfoText")),1)]),h("div",$e,[d(b,{value:!1,class:"is-unclickable"}),w(" "+f(e.$t("permissionPage-checkboxInfoText1")),1)]),h("div",we,[d(b,{value:!1,class:"Checkbox Checkbox--minus is-unclickable"}),w(" "+f(e.$t("permissionPage-checkboxInfoText2")),1)])])):(F(),x("div",xe,[h("p",null,f(e.$t("permissionPage-descriptionUnauthorised",{currentFirmName:e.currentFirmName})),1),h("div",Be,[d(b,{value:!1,class:"Checkbox Checkbox--plus is-unclickable"}),w(" "+f(e.$t("permissionPage-checkboxInfoText")),1)]),h("div",Ie,[d(b,{value:!1,class:"is-unclickable"}),w(" "+f(e.$t("permissionPage-checkboxInfoTextUnauthorised")),1)]),h("div",Le,f(e.$t("permissionPage-InfoText",{complianceFirmName:e.complianceFirmName})),1)]))]),_:1},8,["routeName"]),d(k,{class:"TabStrip TabStrip--lined",selected:e.currentTab,tabs:e.tabs,onSelect:e.selectTab},{"tab-1":C(()=>[h("h3",Ee,f(e.$t("permissionPage-headerTitleText")),1),d(A,{"permission-groups":e.mortgageBrokerPermissions,"permissions-from-fca":e.subPermissionsFromFca,"customer-permissions":e.customerSubPermissions,authorised:e.isFirmAuthorised,onChange:e.updatePermission},null,8,["permission-groups","permissions-from-fca","customer-permissions","authorised","onChange"])]),"tab-2":C(()=>[h("h3",Re,f(e.$t("permissionPage-headerTitleText1")),1),d(A,{"permission-groups":e.insuranceBrokerPermissions,"permissions-from-fca":e.subPermissionsFromFca,"customer-permissions":e.customerSubPermissions,authorised:e.isFirmAuthorised,onChange:e.updatePermission},null,8,["permission-groups","permissions-from-fca","customer-permissions","authorised","onChange"])]),"tab-3":C(()=>[h("h3",Ge,f(e.$t("permissionPage-headerTitleText2")),1),d(A,{"permission-groups":e.otherPermissions,"permissions-from-fca":e.subPermissionsFromFca,"customer-permissions":e.customerSubPermissions,authorised:e.isFirmAuthorised,onChange:e.updatePermission},null,8,["permission-groups","permissions-from-fca","customer-permissions","authorised","onChange"])]),_:1},8,["selected","tabs","onSelect"])],64)}const Ue=z(ke,[["render",Me],["__scopeId","data-v-79223fd0"]]);export{Ue as default};