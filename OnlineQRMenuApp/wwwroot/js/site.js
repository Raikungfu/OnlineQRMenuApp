function login(event, actionLink) {
    event.preventDefault();
    const formData = new URLSearchParams(new FormData(document.getElementById('loginForm'))).toString();

    fetch(actionLink, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData
    }).then(response => {
        if (response.ok) {
            return response.json();
        } else {
            throw new Error('Network response was not ok.');
        }
    }).then(data => {
        const token = data.token;
        if (token) {
            localStorage.setItem('token', token);
            window.location.href = '/';
        }
    }).catch(error => {
        console.error('Error during login:', error);
    });
}