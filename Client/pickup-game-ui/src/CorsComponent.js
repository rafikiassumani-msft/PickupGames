import React from "react"

const CorsComponent = () => {

    React.useEffect(() => {
       const postt = async () => {
           return await postDataJson();
       }   
       postt();
    })


    const postDataJson = async () => {
        return await fetch("https://localhost:7287/users/9", {
            method: "PUT",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                firstName: "Raf",
                lastName: "Doe",
                password: "microsoft123",
                email: "rafdoe@gmail.com"
            })
        })
    }

    const postDataForm = async () => {
        var data = new URLSearchParams();
        data.append('name', 'test@gmail.com');
        return await fetch("https://localhost:61748/api/Values/test", {
            method: "POST",
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },

            body: data
        })
    }

    return <h1> CORS test </h1>
}

export default CorsComponent;