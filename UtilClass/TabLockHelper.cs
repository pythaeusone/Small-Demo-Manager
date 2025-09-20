using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallDemoManager.UtilClass
{
    /// <summary>
    /// Provides helper methods to lock and unlock <see cref="TabPage"/> objects.
    /// Locked tabs can be tracked and retrieved later, ensuring certain tabs remain protected
    /// from user interaction or modification until explicitly unlocked.
    /// </summary>
    public static class TabLockHelper
    {
        /// <summary>
        /// A collection of currently locked <see cref="TabPage"/> instances.
        /// </summary>
        private static List<TabPage> lockedTabs = new List<TabPage>();

        /// <summary>
        /// Locks the given <see cref="TabPage"/> by adding it to the internal list,
        /// preventing it from being accessed or modified unintentionally.
        /// </summary>
        /// <param name="tab">The tab page to lock.</param>
        public static void LockTab(TabPage tab)
        {
            if (!lockedTabs.Contains(tab))
            {
                lockedTabs.Add(tab);
            }
        }

        /// <summary>
        /// Unlocks the given <see cref="TabPage"/> by removing it from the locked list,
        /// allowing normal interaction again.
        /// </summary>
        /// <param name="tab">The tab page to unlock.</param>
        public static void UnlockTab(TabPage tab)
        {
            if (lockedTabs.Contains(tab))
            {
                lockedTabs.Remove(tab);
            }
        }

        /// <summary>
        /// Returns all currently locked <see cref="TabPage"/> instances.
        /// </summary>
        /// <returns>An array of locked tab pages.</returns>
        public static TabPage[] GetLockedTabs()
        {
            return lockedTabs.ToArray();
        }
    }
}
