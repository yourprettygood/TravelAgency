async function postJson(url, data) {
    const res = aw  ait fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });
    const json = await res.json().catch(() => ({}));
    if (!res.ok || json.ok === false) throw new Error(json.error || `Ошибка ${res.status}`);
    return json;
}

document.addEventListener("DOMContentLoaded", () => {
    const loginForm = document.getElementById("loginForm");
    if (loginForm) {
        loginForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const fd = new FormData(loginForm);
            try {
                const r = await postJson("/api/auth/login", {
                    email: fd.get("email"),
                    password: fd.get("password")
                });
                // "Запомнить меня" — сохраняем токен только если отмечено
                const remember = document.getElementById("rememberMe")?.checked;
                if (remember && r.token) localStorage.setItem("auth_token", r.token);

                alert("Вход выполнен");
                // Закрыть модалку (Bootstrap 5)
                const modalEl = document.getElementById("loginModal");
                if (modalEl) bootstrap.Modal.getOrCreateInstance(modalEl).hide();
                loginForm.reset();
            } catch (err) { alert(err.message); }
        });
    }

    const registerForm = document.getElementById("registerForm");
    if (registerForm) {
        registerForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const fd = new FormData(registerForm);
            try {
                const r = await postJson("/api/auth/register", {
                    name: fd.get("name"),
                    email: fd.get("email"),
                    password: fd.get("password")
                });
                alert(r.message || "Регистрация успешна");
                const modalEl = document.getElementById("registerModal");
                if (modalEl) bootstrap.Modal.getOrCreateInstance(modalEl).hide();
                registerForm.reset();
            } catch (err) { alert(err.message); }
        });
    }
});
