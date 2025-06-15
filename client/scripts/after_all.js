conn.start()
.then(()=>{
    conn.invoke("GetMyData", token)
        .catch(err => console.error(err));
})
.catch(err => console.error(err));