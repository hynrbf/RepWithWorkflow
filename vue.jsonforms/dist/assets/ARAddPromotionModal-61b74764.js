import{_ as W}from"./ModalComponent.vue_vue_type_script_setup_true_lang-fe9b98ed.js";import{j as $,C as S,b1 as X,Z as P,o as y,q as K,w as d,n as o,H as L,a2 as R,G as _,P as t,r as w,x as Y,$ as ee,m as U,y as A,A as oe,L as te,b8 as ne,cM as ae,Q as le,U as se,a_ as ie,ba as de,aU as re,O as ue,a5 as pe,a6 as ce,a3 as me,b6 as _e,_ as ve}from"./index-9d6d511f.js";import{_ as ge}from"./OverlayLoader-e5a9021a.js";import{_ as fe,a as be}from"./KendoNumericInputComponent-fbc7c250.js";import{_ as ye}from"./KendoOfficerSelectComponent-a3474693.js";import{_ as he}from"./MediaLabelComponent-feae136c.js";import{u as Ce}from"./ARMarketingAndFinancialPromotions-9f95979b.js";import"./StatusLabelComponent.vue_vue_type_script_setup_true_lang-90f1f182.js";import"./GridCardTable-248623f7.js";import"./DateTimeComponent-2c5db54c.js";const Me=$({__name:"AROutletDropdown",props:{id:{}},setup(r){const v=r,{id:h}=S(v),l=Ce(),{mediaMarketingOutlets:C}=X(l),c=P(()=>C.value.filter(i=>!i.archived).map(i=>({label:i.name,value:i.id}))),m=i=>l.getMediaMarketingOutlet(i);return(i,g)=>{const M=he,u=R;return y(),K(u,{name:"mediaOutlet",id:L(h),label:"Media Outlet","data-items":c.value,"value-primitive":""},{display:d(({value:p})=>{var f;return[o(M,{id:`${(f=m(p.value))==null?void 0:f.platform}`,text:p.label,iconSize:32},null,8,["id","text"])]}),_:1},8,["id","data-items"])}}}),T=r=>(pe("data-v-1387f1b3"),r=r(),ce(),r),Oe={class:"row gy-1"},ke={class:"col-lg-6"},we={class:"col-lg-6"},Pe={class:"col-lg-6"},xe={class:"col-lg-6"},Ue={class:"col-lg-6"},Ae={class:"col-lg-6"},$e={class:"col-lg-6"},Se={class:"col-lg-6"},Ke={class:"col-lg-6"},Le={class:"col-lg-12"},Re={class:"ms-auto"},Te=T(()=>t("b",null,"+ Upload Content",-1)),Ne=[Te],Ie={key:0,class:"col-lg-12"},De=T(()=>t("p",{class:"font-size-sm mb-2"}," Upload Files (Accepted file types: jpg, png, mp4, mov, doc, pdf. Maximum 25 MB file size) ",-1)),Fe={key:1,class:"col-lg-12"},Be={class:"text-right"},Ee=$({__name:"ARAddPromotionModal",props:{initialValues:{},outlets:{default:()=>[]},saving:{type:Boolean},promotionTypeOptions:{default:()=>[]}},emits:["submit"],setup(r,{emit:v}){const h=r,{initialValues:l}=S(h),C=_(),c=_(),m=_([]),i=e=>{m.value=[...m.value,{label:e,value:e}]},g=e=>["authorised-3rd-party","non-authorised-3rd-party"].includes(e),M=P(()=>me.getRemunerationMethods().map(e=>({label:e,value:e}))),u=_(!1),p=_(!1),f=()=>{c.value.reset(),u.value=!1},O=P(()=>!_e(l==null?void 0:l.value)),N=e=>{e.media=(e.media||[]).map(a=>({id:a.uid,name:a.name,extension:a.extension,size:a.size,url:a.uploadedUrl})),O?v("submit",{...(l==null?void 0:l.value)||{},...e}):v("submit",{...e})},s=e=>`${oe.arMarketingAndFinancialPromotionsRoute}-addPromotionalModal${e}`.replace(/\s+/g,"").replace("/","");return(e,a)=>{const I=te,D=ne,x=ye,k=R,F=ae,B=fe,E=le,V=se,z=be,q=ie,j=de,G=re,H=ge,Q=ue,Z=W;return y(),K(Z,{ref_key:"modalElement",ref:C,title:e.$t(O.value?"marketingFinancialPage-editPost":"marketingFinancialPage-newPost"),width:"960","auto-scroll-content":"",onClose:f},{footer:d(()=>[t("div",Be,[o(Q,{type:"button","theme-color":"primary",disabled:!p.value||e.saving,onClick:a[2]||(a[2]=n=>c.value.submit())},{default:d(()=>[w(Y(O.value?"Save Changes":"Save & Add Post"),1)]),_:1},8,["disabled"])])]),default:d(()=>[o(H,{loading:e.saving},{default:d(()=>[o(G,{initialValues:L(l),onSubmit:N},{default:d(()=>[o(j,{ref_key:"formElement",ref:c},{default:d(({form:n,updateValue:J})=>[t("div",Oe,[t("div",ke,[o(Me,{id:s("outletDropdown-mediaOutlet"),value:n.values.mediaOutlet},null,8,["id","value"])]),t("div",we,[o(I,null,{default:d(()=>[w(" ")]),_:1}),o(D,{text:"Is this post currently live?",reverse:"",wide:"","model-value":n.values.isLive,"onUpdate:modelValue":b=>J("isLive",b)},null,8,["model-value","onUpdate:modelValue"])]),t("div",Pe,[o(x,{name:"contentOwner",id:s("-contentOwner"),label:"Content Owner",value:n.values.owner},null,8,["id","value"])]),t("div",xe,[o(x,{name:"moderator",id:s("-moderator"),label:"Line Manager of Content Owner",value:n.values.moderator},null,8,["id","value"])]),t("div",Ue,[o(k,{name:"promotion-type",id:s("-promotion-type"),label:"Financial Promotion Type","data-items":e.promotionTypeOptions,value:n.values.type,"value-primitive":"",info:"Please specify whose products/services you will be promoting."},null,8,["id","data-items","value"])]),t("div",Ae,[o(k,{name:"provider",id:s("-provider"),label:"Provider Name","data-items":m.value,value:n.values.provider,disabled:!g(n.values.type),"value-primitive":"",addable:!0,onAddCustom:i},null,8,["id","data-items","value","disabled"])]),t("div",$e,[o(F,{name:"productType",id:s("-productType"),label:"Product Type",pageName:"Marketing and Financial Promotions",disabled:!g(n.values.type),value:n.values.productType},null,8,["id","disabled","value"])]),t("div",Se,[o(k,{name:"remunerationMethod",id:s("-remunerationMethod"),label:"Remuneration Method","data-items":M.value,value:n.values.remunerationMethod,disabled:!g(n.values.type),"value-primitive":""},null,8,["id","data-items","value","disabled"])]),t("div",Ke,[o(B,{class:"NumberInput",name:"approvalDays",id:s("-approvalDays"),label:"Timeline Required for Approval",value:n.values.approvalDays,"default-value":7,"suffix-text":"days"},null,8,["id","value"])]),t("div",Le,[o(E,{name:"contentUrl",id:s("-contentUrl"),label:"Enter URL (If Applicable. Upload content if none)",type:"url","is-required":!1,value:n.values.contentUrl},{"after-label-text":d(()=>[t("div",Re,[w(" No URL? "),t("a",{href:"#",onClick:a[0]||(a[0]=ee(b=>u.value=!0,["prevent"]))},Ne)])]),_:2},1032,["id","value"])]),u.value?(y(),U("div",Ie,[De,o(V,{name:"media",stretched:"",multiple:"",dropzone:"","allowed-extensions":[".jpg",".png",".mp4",".mov",".doc",".docx",".pdf"],disabled:!1},null,8,["allowed-extensions"])])):A("",!0),u.value?(y(),U("div",Fe,[o(z,{label:"Content Description",id:s("-contentDescription"),"is-required":!1,name:"editorContent.content"},null,8,["id"])])):A("",!0)]),o(q,{class:"ConfirmCheck",modelValue:p.value,"onUpdate:modelValue":a[1]||(a[1]=b=>p.value=b),label:e.$t("marketingFinancialPage-iHerebyConfirm")},null,8,["modelValue","label"])]),_:1},512)]),_:1},8,["initialValues"])]),_:1},8,["loading"])]),_:1},8,["title"])}}});const Xe=ve(Ee,[["__scopeId","data-v-1387f1b3"]]);export{Xe as default};