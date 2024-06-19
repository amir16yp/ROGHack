io.write('Disabling anticheat...\n')

-- Assuming ModuleMgr is defined elsewhere
if ModuleMgr and ModuleMgr.AntiCheatMgr then
    ModuleMgr.AntiCheatMgr.isEnableDetect = false
    ModuleMgr.AntiCheatMgr.isEnableReport = false
    ModuleMgr.AntiCheatMgr.EnableDetect(false)
else
    io.write("Error: AntiCheatMgr not found.\n")
    return
end

local originalOnTick = MgrMgr.RealUpdate
local function onTick()
    local keyCode = UnityEngine.KeyCode.X
    
    if UnityEngine.Input.GetKeyDown(keyCode) then
        local quizMessage = ""
        local questionTable = TableUtil.GetGuildQuizQuestionTable().GetTable()
        for _, question in ipairs(questionTable) do
            if question.Id then
                local correctOption = TableUtil.GetGuildQuizOptionTable().GetRowById(question.CorrectOptId)
                local correctAnswer = correctOption and correctOption.Content or "N/A"
                quizMessage = quizMessage .. string.format("%s:%s\n", question.Content, correctAnswer)
            end
        end
        
        -- Save quizMessage to a file
        local file = io.open("quiz_message.txt", "w") -- open file in write mode
        if file then
            file:write(quizMessage) -- write the message to the file
            file:close() -- close the file
            MgrMgr:GetMgr("TipsMgr").ShowNormalTips("Saved guild quiz answers to quiz_message.txt")
            CommonUI.Dialog.ShowYesNoDlg(true, "MOD", "Open guild quiz answers in notepad?", 
            function()
                -- on confirm, open the file in the web browser     
                local filePath = "file://" .. UnityEngine.Application.dataPath .. '/../quiz_message.txt'
                UnityEngine.Application.OpenURL(filePath)                
            end,
            function()
                -- on cancel
                -- do nothing
            end)
        else
            io.write("Error: Unable to open file for writing.\n")
        end
    end

    originalOnTick()
end

io.write("Hooking game loop...\n")

if MgrMgr and MgrMgr.RealUpdate then
    MgrMgr.RealUpdate = onTick
else
    io.write("Error: RealUpdate not found.\n")
end
