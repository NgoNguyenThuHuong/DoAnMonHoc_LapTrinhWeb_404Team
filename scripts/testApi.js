async function listModels() {
    const apiKey = 'YOUR_API_KEY_HERE';
    const url = `https://generativelanguage.googleapis.com/v1beta/models?key=${apiKey}`;

    try {
        const response = await fetch(url);
        const data = await response.json();
        console.log("Status:", response.status);
        console.log("Data:", JSON.stringify(data, null, 2));
    } catch (e) {
        console.log("Error:", e);
    }
}

listModels();
