async function authedPostJson(url, data) {
    const headers = { "Content-Type": "application/json" };
    const token = localStorage.getItem("auth_token");
    if (token) headers["Authorization"] = `Bearer ${token}`;
    const res = await fetch(url, { method: "POST", headers, body: JSON.stringify(data) });
    const json = await res.json().catch(() => ({}));
    if (!res.ok || json.ok === false) throw new Error(json.error || `Ошибка ${res.status}`);
    return json;
}

document.addEventListener("DOMContentLoaded", () => {
    const msgForm = document.getElementById("messageForm");
    const chatList = document.getElementById("chatList");
    if (msgForm) {
        msgForm.addEventListener("submit", async (e) => {
            e.preventDefault();
            const fd = new FormData(msgForm);
            const text = fd.get("text");
            if (!text || !text.trim()) return alert("Введите сообщение");
            try {
                const r = await authedPostJson("/api/messages", { text });
                if (chatList) {
                    const li = document.createElement("li");
                    li.textContent = `${new Date(r.ts).toLocaleTimeString()} • ${r.text}`;
                    chatList.prepend(li);
                }
                msgForm.reset();
            } catch (err) { alert(err.message); }
        });
    }
});
