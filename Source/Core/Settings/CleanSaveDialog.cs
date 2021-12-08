using PrisonLabor.Core.GameSaves;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Settings
{
    public class CleanSaveDialog : Window
    {
        private string fileName;

        private bool saveBackuped = false;

        public CleanSaveDialog(string fileName)
        {
            absorbInputAroundWindow = true;
            doCloseX = true;

            windowRect = new Rect(0f, 0f, 464f, 232f);

            this.fileName = fileName;
        }

        public override void DoWindowContents(Rect inRect)
        {
            var innerRect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height).ContractedBy(10f);
            GUI.BeginGroup(innerRect);

            var messageRect = new Rect(0f, 0f, innerRect.width, innerRect.height - 90f);
            var cancelButtonRect = new Rect((innerRect.width - 100f) / 2, innerRect.height - 40f, 100f, 40f);
            var backupButtonRect = new Rect(innerRect.width - 110f, innerRect.height - 90f, 100f, 40f);
            var proceedButtonRect = new Rect(innerRect.width - 110f, innerRect.height - 40f, 100f, 40f);

            string dialogMessage = $"{"PrisonLabor_UpgradeSaveDialogMessage".Translate()}\n<color=red>{"PrisonLabor_BackupSaveDialogMessage".Translate()}</color>";
            var listing = new Listing_Standard(GameFont.Medium);
            listing.Begin(messageRect);
            listing.Label(dialogMessage);
            listing.End();
            //GUI.Label(messageRect, dialogMessage);

            Text.Font = GameFont.Small;
            if (Widgets.ButtonText(cancelButtonRect, "CancelButton".Translate()))
            {
                Close();
            }
            if (!saveBackuped && Widgets.ButtonText(backupButtonRect, "PrisonLabor_ButtonBackup".Translate(), true, false, Color.green, !saveBackuped))
            {
                saveBackuped = true;
                SaveCleaner.BackupSavegame(fileName);
            }
            if (Widgets.ButtonText(proceedButtonRect, "PrisonLabor_ButtonProceed".Translate(), true, false, Color.red))
            {
                SaveCleaner.RemoveFromSave(fileName);
                Close();
            }

            GUI.EndGroup();
        }
    }
}
