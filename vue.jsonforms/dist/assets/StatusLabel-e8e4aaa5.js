import{_ as m}from"./StatusLabelComponent.vue_vue_type_script_setup_true_lang-90f1f182.js";import{j as p,C as r,Z as a,o as l,q as _,a3 as i}from"./index-9d6d511f.js";const S=p({__name:"StatusLabel",props:{item:{}},setup(e){const o=e,{item:t}=r(o),n=a(()=>i.getARStatuses().map(({label:s})=>s)[t.value.status??1]),c=a(()=>[0,2,1,1][t.value.status??1]);return(s,f)=>{const u=m;return l(),_(u,{"no-icon":"",status:c.value,text:n.value},null,8,["status","text"])}}});export{S as default};
