import{j as b,bf as v,o as l,m as _,n as t,w as a,H as p,q as r,y as m,bg as f,N as y,O as k,_ as B}from"./index-9d6d511f.js";import{F as u}from"./MediaLabelComponent-feae136c.js";import"./StatusLabelComponent.vue_vue_type_script_setup_true_lang-90f1f182.js";const g={class:"d-inline-flex gap-2"},h=b({__name:"PromotionActions",props:{promotion:{},disabled:{type:Boolean}},emits:["view","remove","approve","reject"],setup(w,{emit:s}){return(e,o)=>{const n=y,i=k,c=v("dev-only");return l(),_("div",g,[t(i,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"View",class:"ActionButton",disabled:e.disabled,onClick:o[0]||(o[0]=d=>s("view",`${e.promotion.id}`))},{default:a(()=>[t(n,{symbol:"eye",size:"20"})]),_:1},8,["disabled"]),e.promotion.approvalStatus===p(u).Pending?(l(),r(i,{key:0,type:"button",size:"small",rounded:"full",shape:"square","theme-color":"success",title:"View",class:"ActionButton",disabled:e.disabled,onClick:o[1]||(o[1]=d=>s("approve",`${e.promotion.id}`))},{default:a(()=>[t(n,{symbol:"check-big",size:"12"})]),_:1},8,["disabled"])):m("",!0),e.promotion.approvalStatus===p(u).Pending?(l(),r(i,{key:1,type:"button",size:"small",rounded:"full",shape:"square","theme-color":"error",title:"View",class:"ActionButton",disabled:e.disabled,onClick:o[2]||(o[2]=d=>s("reject",`${e.promotion.id}`))},{default:a(()=>[t(n,{symbol:"clear",size:"12"})]),_:1},8,["disabled"])):m("",!0),f((l(),r(i,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"Remove",class:"ActionButton",disabled:e.disabled,onClick:o[3]||(o[3]=d=>s("remove",`${e.promotion.id}`))},{default:a(()=>[t(n,{symbol:"trash-bin-remove-6",size:"20"})]),_:1},8,["disabled"])),[[c]])])}}});const A=B(h,[["__scopeId","data-v-0b1dfe37"]]);export{A as default};