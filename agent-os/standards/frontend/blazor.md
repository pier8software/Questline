# Blazor SSR with Tailwind and Alpine.js

## Core Principles

1. **SSR-first approach** - Use Static Server Rendering as the default, minimize JavaScript
2. **Tailwind/DaisyUI for styling** - Utility-first CSS with pre-built components
3. **Alpine.js for client interactivity** - Lightweight JavaScript for simple interactions
4. **Blazor islands for complex needs** - Use `@rendermode InteractiveServer` sparingly

## Tailwind/DaisyUI Setup

### Package References

```xml
<ItemGroup>
    <PackageReference Include="Tailwind.Extensions.AspNetCore" Version="2.*" />
</ItemGroup>
```

### tailwind.config.js

```javascript
/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './**/*.{razor,html,cshtml}'
    ],
    theme: {
        extend: {},
    },
    plugins: [
        require('daisyui'),
    ],
    daisyui: {
        themes: ['light', 'dark'],
    },
}
```

### Alpine.js via CDN

Add to `App.razor` or layout:

```html
<script defer src="https://cdn.jsdelivr.net/npm/alpinejs@3.x.x/dist/cdn.min.js"></script>
```

## Component Patterns

### SSR Component with Tailwind

```razor
@namespace MyApp.Components.Orders

<div class="card bg-base-100 shadow-xl">
    <div class="card-body">
        <h2 class="card-title">@Order.Id</h2>
        <p class="text-sm text-base-content/70">Customer: @Order.CustomerName</p>
        <p class="text-lg font-semibold">@Order.Total.ToString("C")</p>
        <div class="badge @GetStatusBadgeClass()">@Order.Status</div>
    </div>
</div>

@code {
    [Parameter, EditorRequired]
    public OrderViewModel Order { get; set; } = default!;

    private string GetStatusBadgeClass() => Order.Status switch
    {
        "Pending" => "badge-warning",
        "Shipped" => "badge-info",
        "Delivered" => "badge-success",
        _ => "badge-ghost"
    };
}
```

### DaisyUI Component Examples

```razor
@* Buttons *@
<button class="btn btn-primary">Primary</button>
<button class="btn btn-secondary btn-outline">Secondary</button>
<button class="btn btn-error btn-sm">Delete</button>

@* Cards *@
<div class="card bg-base-100 shadow-md">
    <div class="card-body">
        <h2 class="card-title">Card Title</h2>
        <p>Card content</p>
        <div class="card-actions justify-end">
            <button class="btn btn-primary">Action</button>
        </div>
    </div>
</div>

@* Alerts *@
<div class="alert alert-success">
    <span>Operation completed successfully!</span>
</div>

@* Tables *@
<div class="overflow-x-auto">
    <table class="table table-zebra">
        <thead>
            <tr>
                <th>Name</th>
                <th>Status</th>
            </tr>
        </thead>
        <tbody>
            @foreach (var item in Items)
            {
                <tr>
                    <td>@item.Name</td>
                    <td><span class="badge badge-primary">@item.Status</span></td>
                </tr>
            }
        </tbody>
    </table>
</div>
```

## Alpine.js Integration

### Basic x-data Pattern

```razor
<div x-data="{ open: false }">
    <button @click="open = !open" class="btn btn-primary">
        Toggle
    </button>
    <div x-show="open" x-transition class="mt-4 p-4 bg-base-200 rounded-lg">
        Content appears here
    </div>
</div>
```

### Dropdown Menu

```razor
<div x-data="{ open: false }" class="dropdown">
    <button @click="open = !open" class="btn btn-ghost">
        Options
        <svg class="w-4 h-4 ml-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7" />
        </svg>
    </button>
    <ul x-show="open"
        @click.outside="open = false"
        x-transition
        class="dropdown-content menu bg-base-100 rounded-box shadow-lg w-52 p-2">
        <li><a href="/edit">Edit</a></li>
        <li><a href="/duplicate">Duplicate</a></li>
        <li><a href="/delete" class="text-error">Delete</a></li>
    </ul>
</div>
```

### Tabs

```razor
<div x-data="{ activeTab: 'details' }">
    <div class="tabs tabs-boxed">
        <button @click="activeTab = 'details'"
                :class="activeTab === 'details' ? 'tab tab-active' : 'tab'">
            Details
        </button>
        <button @click="activeTab = 'history'"
                :class="activeTab === 'history' ? 'tab tab-active' : 'tab'">
            History
        </button>
        <button @click="activeTab = 'notes'"
                :class="activeTab === 'notes' ? 'tab tab-active' : 'tab'">
            Notes
        </button>
    </div>

    <div class="mt-4">
        <div x-show="activeTab === 'details'">Details content</div>
        <div x-show="activeTab === 'history'">History content</div>
        <div x-show="activeTab === 'notes'">Notes content</div>
    </div>
</div>
```

### Modal Dialog

```razor
<div x-data="{ showModal: false }">
    <button @click="showModal = true" class="btn btn-primary">
        Open Modal
    </button>

    <div x-show="showModal"
         x-transition:enter="transition ease-out duration-200"
         x-transition:leave="transition ease-in duration-150"
         class="modal modal-open">
        <div class="modal-box" @click.outside="showModal = false">
            <h3 class="font-bold text-lg">Confirm Action</h3>
            <p class="py-4">Are you sure you want to proceed?</p>
            <div class="modal-action">
                <button @click="showModal = false" class="btn btn-ghost">Cancel</button>
                <form method="post" asp-page-handler="Confirm">
                    <button type="submit" class="btn btn-primary">Confirm</button>
                </form>
            </div>
        </div>
    </div>
</div>
```

### Form Enhancement

```razor
<form method="post"
      x-data="{ submitting: false }"
      @submit="submitting = true">
    <div class="form-control w-full">
        <label class="label">
            <span class="label-text">Email</span>
        </label>
        <input type="email"
               name="email"
               class="input input-bordered w-full"
               required />
    </div>

    <button type="submit"
            class="btn btn-primary mt-4"
            :disabled="submitting"
            :class="{ 'loading': submitting }">
        <span x-show="!submitting">Submit</span>
        <span x-show="submitting">Submitting...</span>
    </button>
</form>
```

### Accordion

```razor
<div x-data="{ openSection: null }" class="space-y-2">
    @foreach (var (section, index) in Sections.Select((s, i) => (s, i)))
    {
        <div class="collapse collapse-arrow bg-base-200">
            <input type="radio"
                   name="accordion"
                   x-model="openSection"
                   value="@index" />
            <div class="collapse-title text-lg font-medium"
                 @click="openSection = openSection === @index ? null : @index">
                @section.Title
            </div>
            <div class="collapse-content" x-show="openSection === @index" x-collapse>
                <p>@section.Content</p>
            </div>
        </div>
    }
</div>
```

## When to Use Blazor Interactivity

Use `@rendermode InteractiveServer` only when you need:

### Complex Forms with Real-Time Validation

```razor
@rendermode InteractiveServer

<EditForm Model="@_model" OnValidSubmit="HandleSubmit" FormName="order-form">
    <DataAnnotationsValidator />

    <div class="form-control">
        <label class="label"><span class="label-text">Quantity</span></label>
        <InputNumber @bind-Value="_model.Quantity"
                     class="input input-bordered"
                     @oninput="CalculateTotal" />
        <ValidationMessage For="@(() => _model.Quantity)" class="text-error text-sm" />
    </div>

    <div class="mt-4 p-4 bg-base-200 rounded-lg">
        <p class="text-lg">Total: <span class="font-bold">@_calculatedTotal.ToString("C")</span></p>
    </div>

    <button type="submit" class="btn btn-primary mt-4">Place Order</button>
</EditForm>

@code {
    private OrderModel _model = new();
    private decimal _calculatedTotal;

    private void CalculateTotal()
    {
        _calculatedTotal = _model.Quantity * _model.UnitPrice;
    }
}
```

### Real-Time Data (SignalR)

```razor
@rendermode InteractiveServer
@inject IHubContext<NotificationHub> HubContext
@implements IAsyncDisposable

<div class="space-y-2">
    @foreach (var notification in _notifications)
    {
        <div class="alert alert-info">@notification</div>
    }
</div>

@code {
    private List<string> _notifications = [];
    private HubConnection? _hubConnection;

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl(NavigationManager.ToAbsoluteUri("/notificationhub"))
            .Build();

        _hubConnection.On<string>("ReceiveNotification", (message) =>
        {
            _notifications.Insert(0, message);
            InvokeAsync(StateHasChanged);
        });

        await _hubConnection.StartAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (_hubConnection is not null)
        {
            await _hubConnection.DisposeAsync();
        }
    }
}
```

### Heavy Client State Management

```razor
@rendermode InteractiveServer

<div class="grid grid-cols-2 gap-4">
    <div>
        <h3 class="text-lg font-semibold mb-2">Available Items</h3>
        @foreach (var item in _availableItems)
        {
            <div class="p-2 bg-base-200 rounded mb-1 cursor-pointer hover:bg-base-300"
                 @onclick="() => AddToSelected(item)">
                @item.Name
            </div>
        }
    </div>
    <div>
        <h3 class="text-lg font-semibold mb-2">Selected Items (@_selectedItems.Count)</h3>
        @foreach (var item in _selectedItems)
        {
            <div class="p-2 bg-primary/20 rounded mb-1 cursor-pointer hover:bg-primary/30"
                 @onclick="() => RemoveFromSelected(item)">
                @item.Name
            </div>
        }
    </div>
</div>

@code {
    private List<Item> _availableItems = [];
    private List<Item> _selectedItems = [];

    private void AddToSelected(Item item)
    {
        _availableItems.Remove(item);
        _selectedItems.Add(item);
    }

    private void RemoveFromSelected(Item item)
    {
        _selectedItems.Remove(item);
        _availableItems.Add(item);
    }
}
```

## Forms

### Standard HTML Form (Preferred for SSR)

```razor
<form method="post" class="space-y-4">
    <div class="form-control">
        <label class="label">
            <span class="label-text">Customer Name</span>
        </label>
        <input type="text"
               name="customerName"
               value="@Model.CustomerName"
               class="input input-bordered"
               required />
    </div>

    <div class="form-control">
        <label class="label">
            <span class="label-text">Email</span>
        </label>
        <input type="email"
               name="email"
               value="@Model.Email"
               class="input input-bordered"
               required />
    </div>

    <button type="submit" class="btn btn-primary">Submit</button>
</form>
```

### Form with Alpine.js Enhancements

```razor
<form method="post"
      x-data="{
          email: '@Model.Email',
          isValidEmail: false,
          validateEmail() {
              this.isValidEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email);
          }
      }"
      class="space-y-4">

    <div class="form-control">
        <label class="label"><span class="label-text">Email</span></label>
        <input type="email"
               name="email"
               x-model="email"
               @input="validateEmail()"
               :class="email && !isValidEmail ? 'input-error' : ''"
               class="input input-bordered"
               required />
        <label class="label" x-show="email && !isValidEmail">
            <span class="label-text-alt text-error">Please enter a valid email</span>
        </label>
    </div>

    <button type="submit"
            class="btn btn-primary"
            :disabled="!isValidEmail">
        Submit
    </button>
</form>
```

### EditForm for Complex Blazor Scenarios

Only use when you need Blazor's two-way binding and validation:

```razor
@rendermode InteractiveServer

<EditForm Model="@_model" OnValidSubmit="HandleSubmit" FormName="complex-form">
    <DataAnnotationsValidator />
    <ValidationSummary class="text-error text-sm mb-4" />

    <div class="form-control">
        <label class="label"><span class="label-text">Name</span></label>
        <InputText @bind-Value="_model.Name" class="input input-bordered" />
        <ValidationMessage For="@(() => _model.Name)" class="text-error text-sm" />
    </div>

    <button type="submit" class="btn btn-primary mt-4">Save</button>
</EditForm>
```

## File Organization

```
src/
└── MyApp/
    ├── Components/
    │   ├── Layout/
    │   │   ├── MainLayout.razor
    │   │   ├── NavMenu.razor
    │   │   └── Footer.razor
    │   ├── Shared/
    │   │   ├── LoadingSpinner.razor      (SSR, uses Tailwind animation)
    │   │   ├── ConfirmModal.razor        (Alpine.js for show/hide)
    │   │   └── Toast.razor               (Alpine.js for dismiss)
    │   ├── Orders/
    │   │   ├── OrderCard.razor           (SSR)
    │   │   ├── OrderList.razor           (SSR)
    │   │   ├── OrderForm.razor           (Interactive - complex validation)
    │   │   └── OrderStatus.razor         (SSR with Alpine.js dropdown)
    │   └── _Imports.razor
    └── Pages/
        ├── Home.razor
        └── Orders/
            ├── Index.razor               (SSR)
            ├── Details.razor             (SSR with Alpine.js tabs)
            └── Create.razor              (Interactive island)
```

## Guidelines

- **Default to SSR** - Start with static rendering, add interactivity only when needed
- **Use Tailwind utilities** - Prefer utility classes over custom CSS
- **Use DaisyUI components** - Leverage pre-built components for consistency
- **Keep Alpine.js simple** - Use for toggles, tabs, modals; not complex state
- **One interactive island per page** - Avoid multiple interactive regions
- **Preserve SSR benefits** - Fast initial load, SEO-friendly, works without JS
- **Mark required parameters** - Use `[EditorRequired]` for mandatory parameters
- **Initialize with `default!`** - For required reference type parameters
