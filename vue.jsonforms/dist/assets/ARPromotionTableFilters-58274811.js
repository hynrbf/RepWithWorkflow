import{j as S,aa as f,az as L,C as j,aV as T,Z as r,o as d,m as o,n as l,H as e,J as v,y as p,A as D,cL as C,R as q,a3 as k}from"./index-9d6d511f.js";const g=S({__name:"ARPromotionTableFilters",props:{form:{default:()=>({values:{}})},type:{default:"all"}},setup(m){const c=m,b=f(()=>L(()=>import("./ARMediaDropdown-cc52933e.js"),["assets/ARMediaDropdown-cc52933e.js","assets/ARMediaDropdown.vue_vue_type_script_setup_true_lang-15340789.js","assets/index-9d6d511f.js","assets/index-982d5504.css","assets/MediaLabelComponent-feae136c.js","assets/StatusLabelComponent.vue_vue_type_script_setup_true_lang-90f1f182.js","assets/MediaLabelComponent-567bf1c1.css"])),{form:a}=j(c),{currentArFirmName:R}=T(),A=r(()=>k.getPostApprovalTypes().map(n=>({label:n,value:n}))),i=r(()=>[{label:"Live",value:"live"},{label:"Not Live",value:"not live"}]),y=r(()=>[{label:`${R}`,value:"self"},{label:"Authorised Third Party",value:"authorised-3rd-party"},{label:"Non-Authorised Third Party",value:"non-authorised-3rd-party"}]),t=n=>`${D.arMarketingAndFinancialPromotionsRoute}-promotionTableFilters${n}`.replace(/\s+/g,"").replace("/","");return(n,_)=>{const s=C,u=q;return d(),o("div",null,[l(e(b),{id:t("-mediaOutlet"),name:"platform",label:"Media Outlet",value:e(a).values.platform,multiple:"",class:"mb-3"},null,8,["id","value"]),l(s,{name:"promotionType",id:t("-promotionType"),label:"Type","data-items":y.value,value:e(a).values.type,"value-primitive":"",class:"mb-3"},null,8,["id","data-items","value"]),n.type==="pending"?(d(),o(v,{key:0},[l(s,{name:"approvalType",id:t("-approvalType"),label:"Approval Type","data-items":A.value,value:e(a).values.approvalType,class:"mb-3"},null,8,["id","data-items","value"]),l(s,{name:"contentStatus",id:t("-contentStatus"),label:"Content Status","data-items":i.value,value:e(a).values.contentStatus,class:"mb-3"},null,8,["id","data-items","value"]),l(u,{name:"approvalDateRequired",id:t("-approvalDateRequired"),label:"Approval Date Required",value:e(a).values.approvalDateRequired,class:"mb-3"},null,8,["id","value"])],64)):p("",!0),n.type==="approved"?(d(),o(v,{key:1},[l(s,{name:"contentStatusApproved",id:t("-contentStatusApproved"),label:"Content Status","data-items":i.value,value:e(a).values.contentStatus,class:"mb-3"},null,8,["id","data-items","value"]),l(u,{name:"dateApproved",id:t("-dateApproved"),label:"Date Approved",value:e(a).values.dateApproved,class:"mb-3"},null,8,["id","value"]),l(u,{name:"dateLive",id:t("-dateLive"),label:"Date Live",value:e(a).values.dateLive,class:"mb-3"},null,8,["id","value"])],64)):p("",!0),n.type==="rejected"?(d(),o(v,{key:2},[l(s,{name:"contentStatusRejected",id:t("-contentStatusRejected"),label:"Content Status","data-items":i.value,value:e(a).values.contentStatus,class:"mb-3"},null,8,["id","data-items","value"]),l(u,{name:"dateRejected",id:t("-dateRejected"),label:"Date Rejected",value:e(a).values.dateRejected,class:"mb-3"},null,8,["id","value"]),l(u,{name:"dateLive",id:t("-dateLive"),label:"Date Live",value:e(a).values.dateLive,class:"mb-3"},null,8,["id","value"])],64)):p("",!0),n.type==="all"?(d(),o(v,{key:3},[l(s,{name:"contentStatusAll",id:t("-contentStatusAll"),label:"Content Status","data-items":i.value,value:e(a).values.contentStatus,class:"mb-3"},null,8,["id","data-items","value"]),l(u,{name:"approvalRequiredRejected",id:t("-approvalRequiredRejected"),label:"Approval Required Rejected",value:e(a).values.approvalRequiredRejected,class:"mb-3"},null,8,["id","value"]),l(u,{name:"dateApproved",id:t("-dateApproved"),label:"Date Approved",value:e(a).values.dateApproved,class:"mb-3"},null,8,["id","value"]),l(u,{name:"dateRejected",id:t("-dateRejected"),label:"Date Rejected",value:e(a).values.dateRejected,class:"mb-3"},null,8,["id","value"]),l(u,{name:"dateLive",id:t("-dateLive"),label:"Date Live",value:e(a).values.dateLive,class:"mb-3"},null,8,["id","value"])],64)):p("",!0)])}}});export{g as default};