document.addEventListener('DOMContentLoaded', function() {
    const blocks = document.querySelectorAll('pre code.language-mermaid');
    blocks.forEach(block => {
        const pre = block.parentNode;
        const diagramText = block.innerText;
        const mermaidDiv = document.createElement('div');
        mermaidDiv.className = 'mermaid';
        mermaidDiv.textContent = diagramText;
        pre.parentNode.replaceChild(mermaidDiv, pre);
    });
    mermaid.initialize({ startOnLoad: true });
});
