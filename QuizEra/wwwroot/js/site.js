document.addEventListener("DOMContentLoaded", function () {
    const currentPath = window.location.pathname.toLowerCase();

    const navLinks = document.querySelectorAll(".sidebar .nav-link");

    navLinks.forEach(link => {
        const linkPath = link.getAttribute("href");

        if (!linkPath || linkPath === "#") return;

        const formattedLinkPath = linkPath.toLowerCase();

        if (currentPath === formattedLinkPath || (formattedLinkPath !== '/' && currentPath.startsWith(formattedLinkPath))) {
            navLinks.forEach(l => l.classList.remove("active"));
            link.classList.add("active");
        }
    });
});