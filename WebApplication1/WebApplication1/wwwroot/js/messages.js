// wwwroot/js/messages.js

// ===== helpers =====
function setLoading(btnEl, isLoading, textIdle, textLoading) {
    if (!btnEl) return;
    btnEl.disabled = !!isLoading;
    btnEl.dataset.textIdle = btnEl.dataset.textIdle || textIdle || btnEl.textContent || "";
    btnEl.dataset.textLoading = textLoading || "Отправляем...";
    btnEl.textContent = isLoading ? btnEl.dataset.textLoading : btnEl.dataset.textIdle;
}

// Показ уведомлений (использует showToast из auth.js, но если его нет — fallback на alert)
function notify(msg, type = "info") {
    if (typeof window.showToast === "function") {
        window.showToast(msg, type);
    } else {
        alert(msg);
    }
}

async function authedPostJson(url, data) {
    const headers = { "Content-Type": "application/json" };
    const token = localStorage.getItem("auth_token");
    if (token) headers["Authorization"] = `Bearer ${token}`;

    const res = await fetch(url, { method: "POST", headers, body: JSON.stringify(data) });
    const json = await res.json().catch(() => ({}));
    if (!res.ok || json.ok === false) {
        const msg = json.error || `Ошибка ${res.status}`;
        throw new Error(msg);
    }
    return json;
}

// ===== main =====
document.addEventListener("DOMContentLoaded", () => {
    const msgForm = document.getElementById("messageForm");
    const chatList = document.getElementById("chatList");
    if (!msgForm) return;

    const submitBtn = msgForm.querySelector('button[type="submit"]');

    msgForm.addEventListener("submit", async (e) => {
        e.preventDefault();

        const fd = new FormData(msgForm);
        const text = (fd.get("text") || "").toString();

        if (!text.trim()) {
            notify("Введите сообщение", "warning");
            return;
        }

        try {
            setLoading(submitBtn, true, submitBtn?.textContent || "Отправить", "Отправляем...");
            const r = await authedPostJson("/api/messages", { text });

            // визуально добавим сообщение в список
            if (chatList) {
                const li = document.createElement("li");
                li.textContent = `${new Date(r.ts).toLocaleTimeString()} • ${r.text}`;
                chatList.prepend(li);
            }

            notify("Сообщение отправлено", "success");
            msgForm.reset();
        } catch (err) {
            notify(err.message || "Ошибка отправки", "danger");
        } finally {
            setLoading(submitBtn, false);
        }
    });
});
