const apiUrl = 'https://localhost:5142/topics';
const fetchButton = document.getElementById('fetch-topics');


fetchButton.addEventListener('onclick', function (event) {
    event.preventDefault();
    console.log('Hello');
    fetch(apiUrl)
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not OK');
        }

        return response.json();
    })
    .then((data) => {
        console.log(JSON.stringify(data, null, 2));
    })
    .catch(error => {
        console.error('Error: ' + error);
    });
})

