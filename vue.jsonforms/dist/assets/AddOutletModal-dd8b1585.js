import{_ as k}from"./ModalComponent.vue_vue_type_script_setup_true_lang-fe9b98ed.js";import{j as $,C as A,G as u,o as w,q as B,w as o,P as M,n as e,r as E,H as F,A as K,Q as N,ba as V,aU as P,O as S}from"./index-9d6d511f.js";import{_ as U}from"./OverlayLoader-e5a9021a.js";import{_ as R}from"./KendoOfficerSelectComponent-a3474693.js";import{_ as h}from"./MediaDropdown.vue_vue_type_script_setup_true_lang-ff62bd80.js";import"./MediaLabelComponent-feae136c.js";import"./StatusLabelComponent.vue_vue_type_script_setup_true_lang-90f1f182.js";const q={class:"text-right"},T=$({__name:"AddOutletModal",props:{initialValues:{},saving:{type:Boolean}},emits:["submit"],setup(d,{emit:c}){const p=d,{initialValues:l}=A(p),_=u(),i=u(),f=n=>{c("submit",{...(l==null?void 0:l.value)||{},...n})},s=n=>`${K.arMarketingAndFinancialPromotionsRoute}-addOutletModal${n}`.replace(/\s+/g,"").replace("/","");return(n,a)=>{const m=N,v=R,b=U,g=V,C=P,y=S,O=k;return w(),B(O,{ref_key:"modalElement",ref:_,title:n.$t("marketingFinancialPage-addMediaMarketingOutlet"),onClose:a[1]||(a[1]=t=>i.value.reset())},{footer:o(()=>[M("div",q,[e(y,{type:"button","theme-color":"primary",disabled:n.saving,onClick:a[0]||(a[0]=t=>i.value.submit())},{default:o(()=>[E(" Save & Add ")]),_:1},8,["disabled"])])]),default:o(()=>[e(C,{initialValues:F(l),onSubmit:f},{default:o(()=>[e(g,{ref_key:"formElement",ref:i},{default:o(({form:t})=>[e(b,{loading:n.saving},{default:o(()=>{var r;return[e(h,{id:s("-mediaOutlet"),name:"platform",value:(r=t.values)==null?void 0:r.platform,class:"mb-3"},null,8,["id","value"]),e(m,{id:s("-accountName"),name:"name",label:"Account Name",value:t.values.name,class:"mb-3"},null,8,["id","value"]),e(m,{id:s("-accountUrl"),type:"url",name:"url",label:"Account URL",value:t.values.url,class:"mb-3"},null,8,["id","value"]),e(v,{id:s("-primaryOwner"),name:"owner",label:"Primary Content Owner",value:t.values.owner},null,8,["id","value"])]}),_:2},1032,["loading"])]),_:1},512)]),_:1},8,["initialValues"])]),_:1},8,["title"])}}});export{T as default};