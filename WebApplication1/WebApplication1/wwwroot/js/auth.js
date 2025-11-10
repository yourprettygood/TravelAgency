// wwwroot/js/auth.js

// ========== настройки поведения ==========
const SEND_EVEN_IF_INVALID = false;
// false (по умолчанию): при невалидной форме запрос НЕ уходит (валидация на клиенте + toast)
// true: запрос уходит всегда, даже если форма не прошла клиентскую проверку (увидишь ошибку сервера в Network)

// ========== helpers ==========
function $(id) { return document.getElementById(id); }

function setFieldError(inputEl, msg) {
    if (!inputEl) return;
    inputEl.classList.add("is-invalid");
    let fb = inputEl.nextElementSibling;
    if (!fb || !fb.classList.contains("invalid-feedback")) {
        fb = document.createElement("div");
        fb.className = "invalid-feedback";
        inputEl.insertAdjacentElement("afterend", fb);
    }
    fb.textContent = msg || "";
}

function clearFieldError(inputEl) {
    if (!inputEl) return;
    inputEl.classList.remove("is-invalid");
    const fb = inputEl.nextElementSibling;
    if (fb && fb.classList.contains("invalid-feedback")) fb.textContent = "";
}

const isValidEmail = (email) => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(email || "").trim());

function setLoading(btnEl, isLoading, textIdle, textLoading) {
    if (!btnEl) return;
    btnEl.disabled = !!isLoading;
    btnEl.dataset.textIdle = btnEl.dataset.textIdle || textIdle || btnEl.textContent || "";
    btnEl.dataset.textLoading = textLoading || "Загрузка...";
    btnEl.textContent = isLoading ? btnEl.dataset.textLoading : btnEl.dataset.textIdle;
}

// Toast-уведомления (Bootstrap 5)
(function initToast() {
    if (window.showToast) return;
    function ensureContainer() {
        let c = document.getElementById("toastContainer");
        if (!c) {
            c = document.createElement("div");
            c.id = "toastContainer";
            c.className = "toast-container position-fixed top-0 end-0 p-3";
            document.body.appendChild(c);
        }
        return c;
    }
    window.showToast = function (message, type = "info", delay = 3500) {
        try {
            const container = ensureContainer();
            const el = document.createElement("div");
            const bg = type === "success" ? "text-bg-success"
                : type === "danger" ? "text-bg-danger"
                    : type === "warning" ? "text-bg-warning"
                        : "text-bg-secondary";
            el.className = `toast align-items-center ${bg} border-0`;
            el.setAttribute("role", "alert");
            el.setAttribute("aria-live", "assertive");
            el.setAttribute("aria-atomic", "true");
            el.innerHTML = `
        <div class="d-flex">
          <div class="toast-body">${message}</div>
          <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>`;
            container.appendChild(el);
            const t = new bootstrap.Toast(el, { delay });
            t.show();
            el.addEventListener("hidden.bs.toast", () => el.remove());
        } catch {
            alert(message);
        }
    };
})();

// ========== fetch helper ==========
async function postJson(url,     data) {
    const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(data)
    });
    const json = await res.json().catch(() => ({}));
    if (!res.ok || json.ok === false) throw new Error(json.error || `Ошибка ${res.status}`);
    return json;
}

// ========== init ==========
document.addEventListener("DOMContentLoaded", () => {
    // --- LOGIN ---
    const loginForm = $("loginForm");
    const loginBtn = $("loginSubmitBtn") || (loginForm ? loginForm.querySelector('button[type="submit"]') : null);

    if (loginForm) {
        loginForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const fd = new FormData(loginForm);

            const emailEl = $("loginEmail");
            const passEl = $("loginPassword");
            clearFieldError(emailEl); clearFieldError(passEl);

            const email = (fd.get("email") || "").toString();
            const password = (fd.get("password") || "").toString();

            // клиентская проверка: email обязателен и валиден, пароль >= 8
            let valid = true;
            if (!email.trim()) { setFieldError(emailEl, "Введите email"); valid = false; }
            else if (!isValidEmail(email)) { setFieldError(emailEl, "Некорректный email"); valid = false; }

            if (!password.trim()) { setFieldError(passEl, "Введите пароль"); valid = false; }
            else if (password.length < 8) { setFieldError(passEl, "Минимум 8 символов"); valid = false; }

            if (!valid && !SEND_EVEN_IF_INVALID) {
                showToast("Проверьте форму: исправьте подсвеченные поля", "warning");
                return; // не шлём fetch при невалидной форме
            }

            try {
                setLoading(loginBtn, true, "Войти", "Входим...");
                const r = await postJson("/api/auth/login", { email, password });

                // "Запомнить меня" — сохраняем токен только если отмечено
                const remember = $("rememberMe")?.checked;
                if (remember && r.token) localStorage.setItem("auth_token", r.token);

                showToast("Вход выполнен", "success");

                // Закрыть модалку
                const modalEl = $("loginModal");
                if (modalEl) bootstrap.Modal.getOrCreateInstance(modalEl).hide();
                loginForm.reset();
            } catch (err) {
                showToast(err.message || "Ошибка входа", "danger");
            } finally {
                setLoading(loginBtn, false);
            }
        });
    }

    // --- REGISTER ---
    const registerForm = $("registerForm");
    const registerBtn = $("registerSubmitBtn") || (registerForm ? registerForm.querySelector('button[type="submit"]') : null);

    if (registerForm) {
        registerForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const fd = new FormData(registerForm);

            const nameEl = $("regName");
            const emailEl = $("regEmail");
            const passEl = $("regPassword");
            const pass2El = $("regPassword2");

            clearFieldError(nameEl); clearFieldError(emailEl);
            clearFieldError(passEl); clearFieldError(pass2El);

            const name = (fd.get("name") || "").toString();
            const email = (fd.get("email") || "").toString();
            const password = (fd.get("password") || "").toString();
            const passwordConfirm = (fd.get("passwordConfirm") || pass2El?.value || "").toString();

            // клиентская проверка: email валиден, пароль >= 8, совпадение паролей
            let valid = true;
            if (!email.trim()) { setFieldError(emailEl, "Введите email"); valid = false; }
            else if (!isValidEmail(email)) { setFieldError(emailEl, "Некорректный email"); valid = false; }

            if (!password.trim()) { setFieldError(passEl, "Введите пароль"); valid = false; }
            else if (password.length < 8) { setFieldError(passEl, "Минимум 8 символов"); valid = false; }

            if (!passwordConfirm.trim()) { setFieldError(pass2El, "Повторите пароль"); valid = false; }
            else if (password !== passwordConfirm) { setFieldError(pass2El, "Пароли не совпадают"); valid = false; }

            if (!valid && !SEND_EVEN_IF_INVALID) {
                showToast("Проверьте форму: исправьте подсвеченные поля", "warning");
                return; // не шлём fetch при невалидной форме
            }

            try {
                setLoading(registerBtn, true, "Создать аккаунт", "Создаём...");
                const r = await postJson("/api/auth/register", { name, email, password, passwordConfirm });

                showToast(r.message || "Регистрация успешна", "success");
                const modalEl = $("registerModal");
                if (modalEl) bootstrap.Modal.getOrCreateInstance(modalEl).hide();
                registerForm.reset();
            } catch (err) {
                showToast(err.message || "Ошибка регистрации", "danger");
            } finally {
                setLoading(registerBtn, false);
            }
        });
    }
});
