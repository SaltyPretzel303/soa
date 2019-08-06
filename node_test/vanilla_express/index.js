const express=require("express");
const express_app=express();
const port=4000;

express_app.get("/",(req,res)=>{
	res.send("Hello from vanilla express server running on port: "+port);
});

express_app.listen(port,()=>{
	console.log("Server is up and running on port: "+port);
});