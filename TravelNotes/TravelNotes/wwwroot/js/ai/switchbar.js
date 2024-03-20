if (!CSS.supports("timeline-scope", "--foo")) {
    document.body.style.setProperty("--range", range.value);
    range.oninput = () => {
        document.body.style.setProperty("--range", range.value);
    };
}