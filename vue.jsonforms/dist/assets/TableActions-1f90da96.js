import{j as _,o as c,m as p,n as t,w as l,N as d,O as m,_ as u}from"./index-9d6d511f.js";const r={class:"d-inline-flex gap-2"},f=_({__name:"TableActions",props:{item:{}},emits:["view","edit"],setup(b,{emit:o}){return(n,e)=>{const s=d,i=m;return c(),p("div",r,[t(i,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"View",class:"ActionButton",onClick:e[0]||(e[0]=a=>o("view",`${n.item.id}`))},{default:l(()=>[t(s,{symbol:"eye",size:"20"})]),_:1}),t(i,{type:"button",size:"small",rounded:"full",shape:"square","theme-color":"light",title:"Remove",class:"ActionButton",onClick:e[1]||(e[1]=a=>o("edit",`${n.item.id}`))},{default:l(()=>[t(s,{symbol:"edit-pen",size:"20"})]),_:1})])}}});const y=u(f,[["__scopeId","data-v-38329abc"]]);export{y as default};
