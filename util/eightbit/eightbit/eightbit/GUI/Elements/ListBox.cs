using System.Collections.Generic;
using Core.Input;
using Microsoft.Xna.Framework;
using Core.GUI.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace eightbit.GUI.Elements
{
    public class ListBox<T> : WidgetBase<ListBoxRenderRule<T>> where T : class
    {
        public ListBox(int x, int y, int width, int height, List<T> items = null)
        {
            Area = new Rectangle(x, y, width, height);
            if (items != null)
                SetItems(items);
        }

        private int? m_SelectedIndex;
        private List<T> m_Items = new List<T>();
        Core.GUI.Content.ScrollBars m_Scroll;

        /// <summary>
        /// Get the item currently selected. If no item has been selected return null.
        /// </summary>
        public T SelectedItem
        {
            get
            {
                return m_SelectedIndex.HasValue ? (T)m_Items[m_SelectedIndex.Value] : null;
            }
        }

        /// <summary>
        /// The index of the item currently selected. Setting this will call OnSelectionChanged.
        /// </summary>
        public int SelectedIndex
        {
            get
            {
                return m_SelectedIndex.HasValue ? (int)m_SelectedIndex : -1;
            }
            set
            {
                if (value >= 0 && value < m_Items.Count)
                {
                    if (!m_SelectedIndex.HasValue || value != m_SelectedIndex.Value)
                    {
                        m_SelectedIndex = value;
                        if (OnSelectionChanged != null)
                            OnSelectionChanged(this);
                    }
                }
            }
        }

        public void SetSelectionIndexWithoutOnSelectionChanged(int value)
        {
            WidgetEvent we = OnSelectionChanged;
            OnSelectionChanged = null;
            SelectedIndex = value;
            OnSelectionChanged = we;
        }

        public int ScrollPosition
        {
            get { return m_Scroll.ScrollPosition.Y; }
            set { m_Scroll.ScrollPosition = new Point(0, value); }
        }

        /// <summary>
        /// Event fired when ever the selection is changed.
        /// </summary>
        public WidgetEvent OnSelectionChanged { get; set; }

        private void UpdateScrollArea()
        {
            if (m_Scroll != null)
            {
                m_Scroll.OverrideScrollArea(new Rectangle(0, 0, 1, RenderRule.LineSpace * Items.Count));
                if (ScrollPosition + Area.Height > RenderRule.LineSpace * Items.Count)
                    ScrollPosition = RenderRule.LineSpace * Items.Count;
            }
        }

        protected internal override void OnLayout()
        {
            
        }

        protected internal override void OnUpdate()
        {
            if (m_Scroll == null)
            {
                this.AddWidget(m_Scroll = new Core.GUI.Content.ScrollBars());
                UpdateScrollArea();
            }
        }

        protected override ListBoxRenderRule<T> BuildRenderRule()
        {
            return new ListBoxRenderRule<T>(this);
        }
        protected override void Attach() { }

        // ================================================================================
        // Input
        // ================================================================================

        protected internal override void MouseClick(InputEventMouse e)
        {
            if (e.Button == MouseButton.Left)
            {
                int index = RenderRule.ItemAtPosition(new Point(e.Position.X, e.Position.Y));
                if (index >= 0 && index < m_Items.Count)
                    SelectedIndex = index;
            }
        }

        // ================================================================================
        // Item Management
        // ================================================================================

        /// <summary>
        /// Return the Drop Down Box's Items
        /// </summary>        
        public IList<T> Items
        {
            get { return m_Items.AsReadOnly(); }
        }

        public void Add(T item)
        {
            if (m_Items == null)
                m_Items = new List<T>();
            m_Items.Add(item);
            UpdateScrollArea();
        }

        public void RemoveAt(int index)
        {
            if (m_Items == null)
                return;
            if (index < 0 || index >= m_Items.Count)
                return;
            m_Items.RemoveAt(index);
            UpdateScrollArea();
        }

        public void Remove(T item)
        {
            if (m_Items == null)
                m_Items = new List<T>();
            m_Items.Remove(item);
            UpdateScrollArea();
        }

        public void RemoveLast()
        {
            if (m_Items == null)
                return;
            RemoveAt(m_Items.Count - 1);
            UpdateScrollArea();
        }

        /// <summary>
        /// Set the items for the list box.
        /// </summary>
        public void SetItems(IEnumerable<T> items)
        {
            if (m_Items == null)
                m_Items = new List<T>();
            m_Items.Clear();

            if (items != null)
            {
                IEnumerator<T> enumerator = items.GetEnumerator();
                while (enumerator.MoveNext())
                    m_Items.Add(enumerator.Current);
                UpdateScrollArea();
            }
        }
    }
}
