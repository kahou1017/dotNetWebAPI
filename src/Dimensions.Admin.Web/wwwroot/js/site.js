(function () {
    const loadingOverlay = document.getElementById("global-loading-overlay");
    const confirmDialogElement = document.getElementById("global-confirm-dialog");
    const confirmTitleElement = document.getElementById("global-confirm-title");
    const confirmMessageElement = document.getElementById("global-confirm-message");
    const confirmSubmitButton = document.getElementById("global-confirm-submit");

    let pendingForm = null;
    let isConfirmedSubmit = false;
    let confirmDialog = null;

    function showLoadingOverlay() {
        if (loadingOverlay) {
            loadingOverlay.classList.add("is-visible");
            loadingOverlay.setAttribute("aria-hidden", "false");
        }
    }

    function hideLoadingOverlay() {
        if (loadingOverlay) {
            loadingOverlay.classList.remove("is-visible");
            loadingOverlay.setAttribute("aria-hidden", "true");
        }
    }

    if (confirmDialogElement && window.bootstrap) {
        confirmDialog = new bootstrap.Modal(confirmDialogElement);
    }

    document.addEventListener("submit", function (event) {
        const form = event.target;
        if (!(form instanceof HTMLFormElement)) {
            return;
        }

        if (form.classList.contains("js-confirm-submit") && !isConfirmedSubmit) {
            event.preventDefault();

            if (!confirmDialog) {
                const fallback = window.confirm(form.dataset.confirmMessage || "你確定要執行這個操作嗎？");
                if (fallback) {
                    isConfirmedSubmit = true;
                    showLoadingOverlay();
                    form.requestSubmit();
                }
                return;
            }

            pendingForm = form;
            confirmTitleElement.textContent = form.dataset.confirmTitle || "請確認操作";
            confirmMessageElement.textContent = form.dataset.confirmMessage || "你確定要執行這個操作嗎？";
            confirmDialog.show();
            return;
        }

        if (form.classList.contains("js-loading-form")) {
            showLoadingOverlay();
        }
    });

    if (confirmSubmitButton) {
        confirmSubmitButton.addEventListener("click", function () {
            if (!pendingForm) {
                return;
            }

            isConfirmedSubmit = true;
            confirmDialog?.hide();
            showLoadingOverlay();
            pendingForm.requestSubmit();
        });
    }

    if (confirmDialogElement) {
        confirmDialogElement.addEventListener("hidden.bs.modal", function () {
            pendingForm = null;
            isConfirmedSubmit = false;
        });
    }

    window.addEventListener("pageshow", hideLoadingOverlay);
    window.addEventListener("beforeunload", showLoadingOverlay);
})();
