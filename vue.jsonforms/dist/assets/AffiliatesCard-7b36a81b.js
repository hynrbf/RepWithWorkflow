import{j as g,d6 as v,_ as b,o as c,m as d,J as h,K as C,n as t,w as a,P as o,x as _,r as x,bo as w,S as $,O as k,N as A,cz as B,cy as S,a5 as V,a6 as z}from"./index-9d6d511f.js";const I=g({name:"AffiliatesCard",components:{},props:{affiliatesValues:{type:Array,default:()=>[]}},data(){return{}},created(){},watch:{},computed:{AffiliateDetails(){return v}},methods:{}});const N=s=>(V("data-v-beefedbf"),s=s(),z(),s),D={class:"row"},K=N(()=>o("div",{class:"pt-2 flex-grow-0 font-styling"},"Products",-1)),L={class:"flex-grow-0 item-font-styling",id:"cardview-products"},P={class:"d-flex align-items-center justify-content-between gap-2 mt-3"},j={class:"d-flex gap-2"};function q(s,T,E,F,J,O){const f=w,p=$,n=k,i=A,m=B,y=S;return c(),d("div",D,[(c(!0),d(h,null,C(s.affiliatesValues,e=>(c(),d("div",{key:`card-${e.id}`,class:"col-lg-4"},[t(y,{class:"mb-3 cardview-style"},{default:a(()=>[t(m,null,{default:a(()=>{var r,u;return[t(f,{type:"image",rounded:"full",text:`${e==null?void 0:e.details.name} (${e==null?void 0:e.details.firmReferenceNumber})`,"sub-text":`${(r=e.representative)==null?void 0:r.forename} ${(u=e.representative)==null?void 0:u.surname}`},{text:a(({text:l})=>[o("strong",null,_(l),1)]),_:2},1032,["text","sub-text"]),t(p,{class:"d-flex align-items-center mt-3",orientation:"vertical"},{default:a(()=>[K,o("div",L,_(e==null?void 0:e.products),1)]),_:2},1024),o("div",P,[t(n,{class:"button-styling",type:"button","fill-mode":"outline","theme-color":"primary"},{default:a(()=>[x(" View Tasks ")]),_:1}),o("div",j,[t(n,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"View",class:"ActionButton",onClick:l=>s.$emit("view-affiliate",e.id)},{default:a(()=>[t(i,{symbol:"eye",size:"20"})]),_:2},1032,["onClick"]),t(n,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"Pen",class:"ActionButton",onClick:l=>s.$emit("edit-affiliate",e.id)},{default:a(()=>[t(i,{symbol:"edit-pen",size:"20"})]),_:2},1032,["onClick"])])])]}),_:2},1024)]),_:2},1024)]))),128))])}const G=b(I,[["render",q],["__scopeId","data-v-beefedbf"]]);export{G as default};