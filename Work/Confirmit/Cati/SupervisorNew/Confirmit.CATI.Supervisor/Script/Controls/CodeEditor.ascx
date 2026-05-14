<%@ Control Language="C#" CodeBehind="CodeEditor.ascx.cs" Inherits="Confirmit.CATI.Supervisor.Script.Controls.CodeEditor" %>

<controls:TextBox ID="scriptEditor" TextMode="MultiLine" Style="width: 100%; height: 99%; overflow: auto; resize: none" runat="server" onkeyup="scriptChangeHandler()" Visible="false" />

<div class="flex-panel flex-panel-column" style="height: 100%; overflow: hidden;" runat="server" ID="monacoContainer">
    <script src='<%=BaseRelativePath("Client/monaco-editor/min/vs/loader.js")%>'></script>
    <div style="height: 100%" id="scriptContainer"></div>
    <input type="hidden" runat="server" ID="scriptText" name="customScriptText" />
    
<script>
if (!top.__MONACO_EDITOR_EXTRA_LIB__)
{
    YUI().use('io', function (Y) {
        Y.io('<%=BaseRelativePath("Client/catiScripting.d.ts")%>', {
            on : {
                success : function (tx, r) {
                    initEditor(r.responseText)
                    // cache script functions definitions to avoid loading it every time
                    top.__MONACO_EDITOR_EXTRA_LIB__ = r.responseText;
                }
            }
        });
    });
}
else 
{
    initEditor(top.__MONACO_EDITOR_EXTRA_LIB__)
}

function initEditor(catiScripting){
    require.config({ paths: { 'vs': '<%=BaseRelativePath("Client/monaco-editor/min/vs")%>' }});
    require(['vs/editor/editor.main'], function() {
        if (typeof window.monaco !== 'undefined') {
            window.monaco.languages.typescript.javascriptDefaults.setCompilerOptions({
                // allowNonTsExtensions makes jscript.net syntax to not be shown as error, i.e var weekday : String[] = ["a","b"] or var x : String;
                allowNonTsExtensions: true,
                // prevent showing "default" autocomplete for javascript, i.e. alert, setTimeout
                noLib: true,
            });
            
            if (!window.__MONACO_EDITOR_EXTRA_LIB_ADDED__) {
                this.jsExtraLibDisposable = window.monaco.languages.typescript.javascriptDefaults.addExtraLib(
                    catiScripting
                );
                window.__MONACO_EDITOR_EXTRA_LIB_ADDED__ = true; // this flag is needed to prevent loading extra lib when more than one editor is rendered on the page
            }
                
            let options = 
            {
                value: document.getElementById('<%=scriptText.ClientID%>').value,
                language: 'javascript',
                automaticLayout: true,
                fixedOverflowWidgets: true,
            }
            
            if ('<%=LargeScriptFeatures %>' !== 'True') {
                Object.assign(options,
                {
                    lineNumbers: 'off',
                    minimap: {
                        enabled: false
                    },
                    glyphMargin: false,
                    folding: false,
                    lineDecorationsWidth: 3,
                });
            }
            const editor = monaco.editor.create(document.getElementById('scriptContainer'), options);
            
            editor.onKeyUp(scriptChangeHandler);
            
            editor.onDidBlurEditorWidget(function() {
                document.getElementById('<%=scriptText.ClientID%>').value = editor.getValue()
            });
        }
    });
}
    
</script>
</div>
<script>
    function scriptChangeHandler() {
        Common.fireGlobalEvent('ScriptViewChanged');
        stayAlive('<%=KeepSessionUrl%>')
    }
</script>